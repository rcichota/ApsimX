﻿namespace UnitTests.Report
{
    using APSIM.Shared.Utilities;
    using Models;
    using Models.Core;
    using Models.Core.Run;
    using Models.Report;
    using Models.Storage;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using UnitTests.Core;
    using UnitTests.Storage;

    [TestFixture]
    public class ReportTests
    {
        /// <summary>
        /// Template simulations object used to run tests in this class.
        /// </summary>
        private Simulations sims;

        [SetUp]
        public void InitSimulations()
        {
            sims = new Simulations()
            {
                FileName = Path.ChangeExtension(Path.GetTempFileName(), ".apsimx"),
                Children = new List<Model>()
                {
                    new DataStore() { Name = "DataStore" },
                    new Simulation()
                    {
                        Name = "Simulation",
                        Children = new List<Model>()
                        {
                            new Clock()
                            {
                                Name = "Clock",
                                StartDate = new DateTime(2017, 1, 1),
                                EndDate = new DateTime(2017, 1, 10) // January 10
                            },
                            new Summary() { Name = "Summary" },
                            new Zone()
                            {
                                Name = "Zone",
                                Area = 1,
                                Children = new List<Model>()
                                {
                                    new Report()
                                    {
                                        Name = "Report",
                                        VariableNames = new string[]
                                        {
                                            "[Clock].Today.DayOfYear as n",
                                            "sum of [Clock].Today.DayOfYear from [Clock].StartDate to [Clock].Today as TriangularNumbers",
                                            "sum of [Clock].Today.DayOfYear from [Clock].StartOfWeek to [Clock].EndOfWeek as test",
                                            "[Clock].Today.Year as Year",
                                            "sum of [Clock].Today.DayOfYear from 1-Jan to 31-Dec as SigmaDay",
                                        },
                                        EventNames = new string[]
                                        {
                                            "[Clock].DoReport"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
        }

        /// <summary>
        /// This test reproduces a bug where aggregation to [Clock].Today doesn't work, due to
        /// [Clock].Today being evaluated before the simulation starts.
        /// </summary>
        [Test]
        public void EnsureAggregationWorks()
        {
            Clock clock = Apsim.Find(sims, typeof(Clock)) as Clock;
            clock.StartDate = new DateTime(2017, 1, 1);
            clock.EndDate = new DateTime(2017, 1, 10);

            // To test aggregation to [Clock].Today, we generate the first 10
            // triangular numbers by summing [Clock].Today over the first 10 days of the year.
            List<int> triangularNumbers = new List<int>() { 1, 3, 6, 10, 15, 21, 28, 36, 45, 55 };

            // To test aggregation to/from events, we sum day of year from start of week to end of week.
            // The simulation starts in 2017 January 1, which is a Sunday (start of week).
            List<int> weeklyNumbers = new List<int>() { 1, 3, 6, 10, 15, 21, 28, 8, 17, 27 };

            var runner = new Runner(sims);
            runner.Run();

            var storage = sims.Children[0] as IDataStore;
            DataTable data = storage.Reader.GetData("Report", fieldNames: new List<string>() { "n", "TriangularNumbers", "test" });
            List<int> predicted = data.AsEnumerable().Select(x => Convert.ToInt32(x["TriangularNumbers"], CultureInfo.InvariantCulture)).ToList();
            Assert.AreEqual(triangularNumbers, predicted, "Error in report aggregation involving [Clock].Today");

            predicted = data.AsEnumerable().Select(x => Convert.ToInt32(x["test"], CultureInfo.InvariantCulture)).ToList();
            Assert.AreEqual(weeklyNumbers, predicted);
        }

        /// <summary>
        /// This test reproduces a bug where aggregation from 1-Jan to 31-Dec doesn't work properly;
        /// values don't reset after 31-dec, they instead continue aggregating.
        /// </summary>
        [Test]
        public void EnsureYearlyAggregationWorks()
        {
            Clock clock = Apsim.Find(sims, typeof(Clock)) as Clock;
            clock.StartDate = new DateTime(2017, 1, 1);
            clock.EndDate = new DateTime(2019, 1, 1);

            var runner = new Runner(sims);
            runner.Run();

            var storage = sims.Children[0] as IDataStore;
            DataTable data = storage.Reader.GetData("Report", fieldNames: new List<string>() { "Year", "SigmaDay" });
            int finalValFirstYear = int.Parse(data.AsEnumerable().Where(x => int.Parse(x["Year"].ToString()) == 2017).Select(x => x["SigmaDay"]).Last().ToString());
            int firstValSecondYear = int.Parse(data.AsEnumerable().Where(x => int.Parse(x["Year"].ToString()) == 2018).Select(x => x["SigmaDay"]).First().ToString());
            Assert.That(finalValFirstYear > firstValSecondYear, $"Error: Report aggregation from 01-Jan to 31-Dec did not reset after the end date. Final value in first year: {finalValFirstYear}, first value in second year: {firstValSecondYear}");
        }

        [Test]
        public void FactorsTableIsWritten()
        {
            // When report gets an oncommencing it should write a _Factors table to storage.

            var sim = new Simulation();
            sim.Descriptors = new List<SimulationDescription.Descriptor>();
            sim.Descriptors.Add(new SimulationDescription.Descriptor("Experiment", "exp1"));
            sim.Descriptors.Add(new SimulationDescription.Descriptor("SimulationName", "sim1"));
            sim.Descriptors.Add(new SimulationDescription.Descriptor("FolderName", "F"));
            sim.Descriptors.Add(new SimulationDescription.Descriptor("Zone", "z"));
            sim.Descriptors.Add(new SimulationDescription.Descriptor("Cultivar", "cult1"));
            sim.Descriptors.Add(new SimulationDescription.Descriptor("N", "0"));

            var report = new Report()
            {
                VariableNames = new string[0],
                EventNames = new string[0]
            };
            Utilities.InjectLink(report, "simulation", sim);
            Utilities.InjectLink(report, "locator", new MockLocator());
            Utilities.InjectLink(report, "storage", new MockStorage());

            var events = new Events(report);
            events.Publish("StartOfSimulation", new object[] { report, new EventArgs() });

            Assert.AreEqual(MockStorage.tables[0].TableName, "_Factors");
            Assert.AreEqual(Utilities.TableToString(MockStorage.tables[0]),
               "ExperimentName,SimulationName,FolderName,FactorName,FactorValue\r\n" +
               "          exp1,          sim1,         F,  Cultivar,      cult1\r\n" +
               "          exp1,          sim1,         F,         N,          0\r\n");
        }
    }
}