using System.Collections.Generic;

namespace PersistentThrust
{
    // Container for propellant info used by PersistentEngine
    public class PersistentPropellant
    {
        // Fields
        public Propellant propellant;
        public PartResourceDefinition definition;
        public double density;
        public double ratio;
        public double normalizedRatio;

        public double normalizedDemand;
        public double totalEnginesDemand;

        public double maxAmount;
        public double amount;
        public double missionTime;
        public double demandIn;
        public double demandOut;

        // Constructor
        private PersistentPropellant(Propellant p)
        {
            propellant = p;
            definition = PartResourceLibrary.Instance.GetDefinition(propellant.name);
            density = definition.density;
            ratio = propellant.ratio;
        }

        // Calculate demand of this propellant given the total demand of the moduleEngine
        public double CalculateDemand(double demand)
        {
            normalizedDemand = demand * normalizedRatio;

            return normalizedDemand;
        }

        // Static methods

        // Generate list of PersistentPropellant from propellant list
        public static List<PersistentPropellant> MakeList(List<Propellant> plist)
        {
            // Sum of ratios of propellants with mass
            var ratioMassSum = 0.0;
            // Create list of PersistentPropellant and calculate ratioSum & ratioMassSum
            var pplist = new List<PersistentPropellant>();
            foreach (var p in plist)
            {
                var pp = new PersistentPropellant(p);
                pplist.Add(pp);
                if (pp.density > 0)
                    ratioMassSum += pp.ratio;
            }

            // Normalize ratios to ratioMassSum
            if (ratioMassSum > 0)
            {
                foreach (var pp in pplist)
                {
                    pp.normalizedRatio = pp.ratio / ratioMassSum;
                }
            }

            return pplist;
        }
    }

    // Extensions to list of PersistentPropellant
    public static class PPListExtensions
    {
        // Calculate average density from a list of PersistentPropellant
        public static double AverageDensity(this List<PersistentPropellant> pplist)
        {
            double avgDensity = 0;
            foreach (var pp in pplist)
            {
                if (pp.density > 0)
                    avgDensity += pp.normalizedRatio * pp.density;
            }
            return avgDensity;
        }

        // Generate string with list of propellant names for use in moduleEngine GUI
        public static string ResourceNames(this List<PersistentPropellant> pplist)
        {
            var title = "";
            foreach (var pp in pplist)
            {
                // If multiple resources, put | between them
                if (title != string.Empty)
                {
                    title += "|";
                }
                // Add name of resource
                title += pp.propellant.name;
            }
            title += " use";
            return title;
        }

        // Generate string with list of current propellant amounts for use in moduleEngine GUI
        // Give current step size as argument
        public static string ResourceAmounts(this List<PersistentPropellant> pplist, double dT)
        {
            if (dT == 0)
                return "";

            var amounts = "";
            foreach (var pp in pplist)
            {
                // If multiple resources, put | between them
                if (amounts != string.Empty)
                    amounts += "|";

                // Add current amount * dT
                amounts += (pp.propellant.currentAmount / dT).ToString("E3");
            }
            amounts += " U/s";
            return amounts;
        }
    }
}
