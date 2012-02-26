﻿// Accord Unit Tests
// The Accord.NET Framework
// http://accord-net.origo.ethz.ch
//
// Copyright © César Souza, 2009-2012
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Models.Markov;
    using Accord.Statistics.Distributions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Accord.Math;
    using Accord.Statistics.Distributions.Univariate;
    using Accord.Statistics.Models.Markov.Learning;
    using Accord.Statistics.Models.Markov.Topology;
    using Accord.Statistics.Distributions.Fitting;
    using Accord.MachineLearning;
    using Accord.Statistics.Distributions.Multivariate;
    using System;

    [TestClass()]
    public class GenericHiddenMarkovModelTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod()]
        public void ConstructorTest()
        {
            double[,] A;
            double[] pi;

            var hmm = HiddenMarkovModel.CreateGeneric(2, 4);

            A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.5, 0.5 }
            };

            pi = new double[] { 1, 0 };

            var logA = Matrix.Log(A);
            var logPi = Matrix.Log(pi);

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(1, hmm.Dimension);
            Assert.IsTrue(logA.IsEqual(hmm.Transitions));
            Assert.IsTrue(logPi.IsEqual(hmm.Probabilities));




            hmm = HiddenMarkovModel.CreateGeneric(new Forward(2), 4);

            A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.0, 1.0 }
            };

            pi = new double[] { 1, 0 };

            logA = Matrix.Log(A);
            logPi = Matrix.Log(pi);

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(1, hmm.Dimension);
            Assert.IsTrue(logA.IsEqual(hmm.Transitions));
            Assert.IsTrue(logPi.IsEqual(hmm.Probabilities));



            A = new double[,]
            {  
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            GeneralDiscreteDistribution[] B = 
            {  
                new GeneralDiscreteDistribution(0.1, 0.4, 0.5),
                new GeneralDiscreteDistribution(0.6, 0.3, 0.1)
            };

            pi = new double[]
            {
                0.6, 0.4
            };

            hmm = new HiddenMarkovModel<GeneralDiscreteDistribution>(A, B, pi);

            logA = Matrix.Log(A);
            logPi = Matrix.Log(pi);

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(1, hmm.Dimension);
            Assert.IsTrue(logA.IsEqual(hmm.Transitions));
            Assert.IsTrue(logPi.IsEqual(hmm.Probabilities));
            Assert.AreEqual(B, hmm.Emissions);
        }

        [TestMethod()]
        public void ConstructorTest2()
        {

            double[,] A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.5, 0.5 }
            };

            double[] pi = new double[] { 1, 0 };

            var distribution = new MultivariateNormalDistribution(3);
            var hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(2, distribution);

            for (int i = 0; i < hmm.Emissions.Length; i++)
            {
                IDistribution b = hmm.Emissions[i];

                Assert.AreNotSame(distribution, b);
                Assert.IsTrue(b is MultivariateNormalDistribution);

                MultivariateNormalDistribution n = b as MultivariateNormalDistribution;

                Assert.AreEqual(n.Dimension, hmm.Dimension);

                Assert.AreNotEqual(n.Covariance, distribution.Covariance);
                Assert.IsTrue(n.Covariance.IsEqual(distribution.Covariance));

                Assert.AreNotEqual(n.Mean, distribution.Mean);
                Assert.IsTrue(n.Mean.IsEqual(distribution.Mean));
            }

            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(3, hmm.Dimension);
            Assert.AreEqual(2, hmm.Emissions.Length);

            var logA = Matrix.Log(A);
            var logPi = Matrix.Log(pi);

            Assert.IsTrue(logA.IsEqual(hmm.Transitions));
            Assert.IsTrue(logPi.IsEqual(hmm.Probabilities));
        }

        [TestMethod()]
        public void ConstructorTest3()
        {
            double[,] A = new double[,]
            {
                { 0.5, 0.5 },
                { 0.5, 0.5 }
            };

            double[] pi = new double[] { 1, 0 };

            MultivariateNormalDistribution[] emissions = 
            {
                new MultivariateNormalDistribution(new[] { 0.0, 0.1 }, new[,] { {1.0, 0.0}, {1.0, 5.1} }),
                new MultivariateNormalDistribution(new[] { 2.0, 0.0 }, new[,] { {1.1, 0.1}, {1.0, 6.0} }),
            };

            var hmm = new HiddenMarkovModel<MultivariateNormalDistribution>(A, emissions, pi);

            for (int i = 0; i < hmm.Emissions.Length; i++)
            {
                IDistribution b = hmm.Emissions[i];
                IDistribution e = emissions[i];

                Assert.AreEqual(b, e);
            }

            A = Matrix.Log(A);
            pi = Matrix.Log(pi);
            Assert.AreEqual(2, hmm.States);
            Assert.AreEqual(2, hmm.Dimension);
            Assert.AreEqual(2, hmm.Emissions.Length);
            Assert.IsTrue(A.IsEqual(hmm.Transitions));
            Assert.IsTrue(pi.IsEqual(hmm.Probabilities));
        }


        [TestMethod()]
        public void DecodeTest()
        {
            double[,] transitions = 
            {  
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            GeneralDiscreteDistribution[] emissions = 
            {  
                new GeneralDiscreteDistribution(0.1, 0.4, 0.5),
                new GeneralDiscreteDistribution(0.6, 0.3, 0.1)
            };

            double[] initial =
            {
                0.6, 0.4
            };

            var hmm = new HiddenMarkovModel<GeneralDiscreteDistribution>(transitions, emissions, initial);

            double logLikelihood;
            double[] sequence = new double[] { 0, 1, 2 };
            int[] path = hmm.Decode(sequence, out logLikelihood);

            double expected = Math.Log(0.01344);

            Assert.AreEqual(logLikelihood, expected, 1e-10);
            Assert.AreEqual(path[0], 1);
            Assert.AreEqual(path[1], 0);
            Assert.AreEqual(path[2], 0);
        }

        [TestMethod()]
        public void DecodeTest2()
        {
            double[,] transitions = 
            {  
                { 0.7, 0.3 },
                { 0.4, 0.6 }
            };

            double[,] emissions = 
            {  
                { 0.1, 0.4, 0.5 },
                { 0.6, 0.3, 0.1 }
            };

            double[] initial =
            {
                0.6, 0.4
            };

            var hmm = HiddenMarkovModel.CreateGeneric(transitions, emissions, initial);

            double logLikelihood;
            double[] sequence = new double[] { 0, 1, 2 };
            int[] path = hmm.Decode(sequence, out logLikelihood);

            double expected = Math.Log(0.01344);

            Assert.AreEqual(logLikelihood, expected, 1e-10);
            Assert.AreEqual(path[0], 1);
            Assert.AreEqual(path[1], 0);
            Assert.AreEqual(path[2], 0);
        }


        [TestMethod()]
        public void LearnTest5()
        {

            double[][][] sequences = new double[][][] 
            {
                new double[][] { new double[] { 0 }, new double[] { 3 }, new double[] { 1} },
                new double[][] { new double[] { 0 }, new double[] { 2 } },
                new double[][] { new double[] { 1 }, new double[] { 0 }, new double[] { 3 } },
                new double[][] { new double[] { 3 }, new double[] { 4 } },
                new double[][] { new double[] { 0 }, new double[] { 1 }, new double[] { 3 }, new double[] { 5 } },
                new double[][] { new double[] { 0 }, new double[] { 3 }, new double[] { 4 } },
                new double[][] { new double[] { 0 }, new double[] { 1 }, new double[] { 3 }, new double[] { 5 } },
                new double[][] { new double[] { 0 }, new double[] { 1 }, new double[] { 3 }, new double[] { 5 } },
                new double[][] { new double[] { 0 }, new double[] { 1 }, new double[] { 3 }, new double[] { 4 }, new double[] { 5 } },
            };

            var hmm = HiddenMarkovModel.CreateGeneric(3, 6);

            var teacher = new BaumWelchLearning<GeneralDiscreteDistribution>(hmm) { Iterations = 100, Tolerance = 0 };
            double ll = teacher.Run(sequences);

            double l0; hmm.Decode(sequences[0], out l0);
            double l1; hmm.Decode(sequences[1], out l1);
            double l2; hmm.Decode(sequences[2], out l2);

            double pl = System.Math.Exp(ll);
            double p0 = System.Math.Exp(l0);
            double p1 = System.Math.Exp(l1);
            double p2 = System.Math.Exp(l2);

            Assert.AreEqual(0.497856082499102, pl, 1e-6);
            Assert.AreEqual(0.013775201022629, p0, 1e-6);
            Assert.AreEqual(0.016731134938742, p1, 1e-6);
            Assert.AreEqual(0.002028146584280, p2, 1e-6);

            Assert.AreEqual(1, hmm.Dimension);




            double[][] sequences2 = new double[][] 
            {
                new double[] { 0, 3, 1 },
                new double[] { 0, 2 },
                new double[] { 1, 0, 3 },
                new double[] { 3, 4 },
                new double[] { 0, 1, 3, 5 },
                new double[] { 0, 3, 4 },
                new double[] { 0, 1, 3, 5 },
                new double[] { 0, 1, 3, 5 },
                new double[] { 0, 1, 3, 4, 5 },
            };

            hmm = HiddenMarkovModel.CreateGeneric(3, 6);

            teacher = new BaumWelchLearning<GeneralDiscreteDistribution>(hmm) { Iterations = 100 };
            double ll2 = teacher.Run(sequences2);

            double l02; hmm.Decode(sequences2[0], out l02);
            double l12; hmm.Decode(sequences2[1], out l12);
            double l22; hmm.Decode(sequences2[2], out l22);

            Assert.AreEqual(ll, ll2);
            Assert.AreEqual(l0, l02);
            Assert.AreEqual(l1, l12);
            Assert.AreEqual(l2, l22);

            Assert.AreEqual(1, hmm.Dimension);
        }

        [TestMethod()]
        public void LearnTest3()
        {

            double[][] sequences = new double[][] 
            {
                new double[] { 0,1,1,1,1,0,1,1,1,1 },
                new double[] { 0,1,1,1,0,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1         },
                new double[] { 0,1,1,1,1,1,1       },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a new Hidden Markov Model with 3 states
            var hmm = HiddenMarkovModel.CreateGeneric(3, 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<GeneralDiscreteDistribution>(hmm) { Tolerance = 0.0001 };
            double ll = teacher.Run(sequences);

            // Calculate the probability that the given
            //  sequences originated from the model
            double l1; hmm.Decode(new double[] { 0, 1 }, out l1);        // 0.4999
            double l2; hmm.Decode(new double[] { 0, 1, 1, 1 }, out l2);  // 0.1145

            double l3; hmm.Decode(new double[] { 1, 1 }, out l3);        // 0.0000
            double l4; hmm.Decode(new double[] { 1, 0, 0, 0 }, out l4);  // 0.0000

            double l5; hmm.Decode(new double[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 }, out l5); // 0.0002
            double l6; hmm.Decode(new double[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 }, out l6); // 0.0002


            ll = System.Math.Exp(ll);
            l1 = System.Math.Exp(l1);
            l2 = System.Math.Exp(l2);
            l3 = System.Math.Exp(l3);
            l4 = System.Math.Exp(l4);
            l5 = System.Math.Exp(l5);
            l6 = System.Math.Exp(l6);

            Assert.AreEqual(0.95151018769760853, ll, 1e-4);
            Assert.AreEqual(0.4999419764097881, l1, 1e-4);
            Assert.AreEqual(0.1145702973735144, l2, 1e-4);
            Assert.AreEqual(0.0000529972606821, l3, 1e-4);
            Assert.AreEqual(0.0000000000000001, l4, 1e-4);
            Assert.AreEqual(0.0002674509390361, l5, 1e-4);
            Assert.AreEqual(0.0002674509390361, l6, 1e-4);

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);

            Assert.AreEqual(1, hmm.Dimension);
        }

        [TestMethod()]
        public void LearnTest6()
        {
            // Continuous Markov Models can operate using any
            // probability distribution, including discrete ones. 

            // In the follwing example, we will try to create a
            // Continuous Hidden Markov Model using a discrete
            // distribution to detect if a given sequence starts
            // with a zero and has any number of ones after that.

            double[][] sequences = new double[][] 
            {
                new double[] { 0,1,1,1,1,0,1,1,1,1 },
                new double[] { 0,1,1,1,0,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1         },
                new double[] { 0,1,1,1,1,1,1       },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
                new double[] { 0,1,1,1,1,1,1,1,1,1 },
            };

            // Create a new Hidden Markov Model with 3 states and
            //  a generic discrete distribution with two symbols
            var hmm = HiddenMarkovModel.CreateGeneric(3, 2);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<GeneralDiscreteDistribution>(hmm)
            {
                Tolerance = 0.0001,
                Iterations = 0
            };

            double ll = Math.Exp(teacher.Run(sequences));

            // Calculate the probability that the given
            //  sequences originated from the model
            double l1 = Math.Exp(hmm.Evaluate(new double[] { 0, 1 }));       // 0.999
            double l2 = Math.Exp(hmm.Evaluate(new double[] { 0, 1, 1, 1 })); // 0.916

            // Sequences which do not start with zero have much lesser probability.
            double l3 = Math.Exp(hmm.Evaluate(new double[] { 1, 1 }));       // 0.000
            double l4 = Math.Exp(hmm.Evaluate(new double[] { 1, 0, 0, 0 })); // 0.000

            // Sequences which contains few errors have higher probabability
            //  than the ones which do not start with zero. This shows some
            //  of the temporal elasticity and error tolerance of the HMMs.
            double l5 = Math.Exp(hmm.Evaluate(new double[] { 0, 1, 0, 1, 1, 1, 1, 1, 1 })); // 0.034
            double l6 = Math.Exp(hmm.Evaluate(new double[] { 0, 1, 1, 1, 1, 1, 1, 0, 1 })); // 0.034


            Assert.AreEqual(0.95151018769760853, ll, 1e-4);
            Assert.AreEqual(0.99996863060890995, l1, 1e-4);
            Assert.AreEqual(0.91667240076011669, l2, 1e-4);
            Assert.AreEqual(0.00002335133758386, l3, 1e-4);
            Assert.AreEqual(0.00000000000000012, l4, 1e-4);
            Assert.AreEqual(0.03423723144322685, l5, 1e-4);
            Assert.AreEqual(0.03423719592053246, l6, 1e-4);

            Assert.IsFalse(Double.IsNaN(ll));
            Assert.IsFalse(Double.IsNaN(l1));
            Assert.IsFalse(Double.IsNaN(l2));
            Assert.IsFalse(Double.IsNaN(l3));
            Assert.IsFalse(Double.IsNaN(l4));
            Assert.IsFalse(Double.IsNaN(l5));
            Assert.IsFalse(Double.IsNaN(l6));

            Assert.IsTrue(l1 > l3 && l1 > l4);
            Assert.IsTrue(l2 > l3 && l2 > l4);
        }

        [TestMethod()]
        public void LearnTest7()
        {
            // Create continuous sequences. In the sequences below, there
            //  seems to be two states, one for values between 0 and 1 and
            //  another for values between 5 and 7. The states seems to be
            //  switched on every observation.
            double[][] sequences = new double[][] 
            {
                new double[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 },
                new double[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 },
                new double[] { 0.1, 7.0, 0.1, 7.0, 0.2, 5.6 },
            };


            // Specify a initial normal distribution for the samples.
            var density = new NormalDistribution();

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<NormalDistribution>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<NormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
            };

            // Fit the model
            double logLikelihood = teacher.Run(sequences);

            // See the probability of the sequences learned
            double a1 = model.Evaluate(new[] { 0.1, 5.2, 0.3, 6.7, 0.1, 6.0 }); // 0.87
            double a2 = model.Evaluate(new[] { 0.2, 6.2, 0.3, 6.3, 0.1, 5.0 }); // 1.00

            // See the probability of an unrelated sequence
            double a3 = model.Evaluate(new[] { 1.1, 2.2, 1.3, 3.2, 4.2, 1.0 }); // 0.00

            double likelihood = Math.Exp(logLikelihood);
            a1 = Math.Exp(a1);
            a2 = Math.Exp(a2);
            a3 = Math.Exp(a3);

            Assert.AreEqual(1.1341500279562791, likelihood, 1e-10);
            Assert.AreEqual(0.8798587580029778, a1, 1e-10);
            Assert.AreEqual(1.0117804233450216, a2, 1e-10);
            Assert.AreEqual(1.8031545195073828E-130, a3, 1e-10);

            Assert.IsFalse(double.IsNaN(logLikelihood));
            Assert.IsFalse(double.IsNaN(a1));
            Assert.IsFalse(double.IsNaN(a2));
            Assert.IsFalse(double.IsNaN(a3));


            Assert.AreEqual(2, model.Emissions.Length);
            var state1 = (model.Emissions[0] as NormalDistribution);
            var state2 = (model.Emissions[1] as NormalDistribution);
            Assert.AreEqual(0.16666666666666, state1.Mean, 1e-10);
            Assert.AreEqual(6.11111111111111, state2.Mean, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Mean));
            Assert.IsFalse(Double.IsNaN(state2.Mean));

            Assert.AreEqual(0.007499999999999, state1.Variance, 1e-10);
            Assert.AreEqual(0.538611111111111, state2.Variance, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Variance));
            Assert.IsFalse(Double.IsNaN(state2.Variance));

            Assert.AreEqual(2, model.Transitions.GetLength(0));
            Assert.AreEqual(2, model.Transitions.GetLength(1));

            var A = Matrix.Exp(model.Transitions);
            Assert.AreEqual(0, A[0, 0], 1e-16);
            Assert.AreEqual(1, A[0, 1], 1e-16);
            Assert.AreEqual(1, A[1, 0], 1e-16);
            Assert.AreEqual(0, A[1, 1], 1e-16);

            Assert.IsFalse(A.HasNaN());
        }

        [TestMethod()]
        public void LearnTest8()
        {
            // Create continuous sequences. In the sequence below, there
            // seems to be two states, one for values equal to 1 and another
            // for values equal to 2.
            double[][] sequences = new double[][] 
            {
                new double[] { 1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2 }             
            };

            // Specify a initial normal distribution for the samples.
            var density = new NormalDistribution();

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<NormalDistribution>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<NormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                // However, we will need to specify a regularization constant as the
                //  variance of each state will likely be zero (all values are equal)
                FittingOptions = new NormalOptions() { Regularization = double.Epsilon }
            };

            // Fit the model
            double likelihood = teacher.Run(sequences);


            // See the probability of the sequences learned
            double a1 = model.Evaluate(new double[] { 1, 2, 1, 2, 1, 2, 1, 2, 1 }); // exp(a1) = inf
            double a2 = model.Evaluate(new double[] { 1, 2, 1, 2, 1 });             // exp(a2) = inf

            // See the probability of an unrelated sequence
            double a3 = model.Evaluate(new double[] { 1, 2, 3, 2, 1, 2, 1 });          // exp(a3) = 0
            double a4 = model.Evaluate(new double[] { 1.1, 2.2, 1.3, 3.2, 4.2, 1.0 }); // exp(a4) = 0


            Assert.AreEqual(double.PositiveInfinity, System.Math.Exp(likelihood));
            Assert.AreEqual(302.59496915947972, a1);
            Assert.AreEqual(168.26234890650207, a2);
            Assert.AreEqual(0.0, Math.Exp(a3));
            Assert.AreEqual(0.0, Math.Exp(a4));

            Assert.AreEqual(2, model.Emissions.Length);
            var state1 = (model.Emissions[0] as NormalDistribution);
            var state2 = (model.Emissions[1] as NormalDistribution);
            Assert.AreEqual(1.0, state1.Mean, 1e-10);
            Assert.AreEqual(2.0, state2.Mean, 1e-10);
            Assert.IsFalse(Double.IsNaN(state1.Mean));
            Assert.IsFalse(Double.IsNaN(state2.Mean));

            Assert.IsTrue(state1.Variance < 1e-30);
            Assert.IsTrue(state2.Variance < 1e-30);

            var A = Matrix.Exp(model.Transitions);
            Assert.AreEqual(2, A.GetLength(0));
            Assert.AreEqual(2, A.GetLength(1));
            Assert.AreEqual(0, A[0, 0]);
            Assert.AreEqual(1, A[0, 1]);
            Assert.AreEqual(1, A[1, 0]);
            Assert.AreEqual(0, A[1, 1]);
        }


        [TestMethod()]
        public void FittingOptionsTest()
        {
            // Create a degenerate problem
            double[][] sequences = new double[][] 
            {
                new double[] { 1,1,1,1,1,0,1,1,1,1 },
                new double[] { 1,1,1,1,0,1,1,1,1,1 },
                new double[] { 1,1,1,1,1,1,1,1,1,1 },
                new double[] { 1,1,1,1,1,1         },
                new double[] { 1,1,1,1,1,1,1       },
                new double[] { 1,1,1,1,1,1,1,1,1,1 },
                new double[] { 1,1,1,1,1,1,1,1,1,1 },
            };

            // Creates a continuous hidden Markov Model with two states organized in a ergodic
            //  topology and an underlying multivariate Normal distribution as density.
            var density = new MultivariateNormalDistribution(1);

            var model = new HiddenMarkovModel<MultivariateNormalDistribution>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier
            var teacher = new BaumWelchLearning<MultivariateNormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                // Configure options for fitting the normal distribution
                FittingOptions = new NormalOptions() { Regularization = 0.0001, }
            };

            // Fit the model. No exceptions will be thrown
            double logLikelihood = teacher.Run(sequences);
            double likelihood = Math.Exp(logLikelihood);

            Assert.AreEqual(47.434837528491286, logLikelihood, 1e-15);
            Assert.IsFalse(double.IsNaN(logLikelihood));

            Assert.AreEqual(0.0001, (teacher.FittingOptions as NormalOptions).Regularization);



            // Try without a regularization constant to get an exception
            bool thrown;

            thrown = false;
            density = new MultivariateNormalDistribution(1);
            model = new HiddenMarkovModel<MultivariateNormalDistribution>(new Ergodic(2), density);
            teacher = new BaumWelchLearning<MultivariateNormalDistribution>(model) { Tolerance = 0.0001, Iterations = 0, };
            Assert.IsNull(teacher.FittingOptions);
            try { teacher.Run(sequences); }
            catch { thrown = true; }
            Assert.IsTrue(thrown);

            thrown = false;
            density = new Accord.Statistics.Distributions.Multivariate.MultivariateNormalDistribution(1);
            model = new HiddenMarkovModel<MultivariateNormalDistribution>(new Ergodic(2), density);
            teacher = new BaumWelchLearning<MultivariateNormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,
                FittingOptions = new NormalOptions() { Regularization = 0 }
            };
            Assert.IsNotNull(teacher.FittingOptions);
            try { teacher.Run(sequences); }
            catch { thrown = true; }
            Assert.IsTrue(thrown);
        }


        [TestMethod()]
        public void PredictTest()
        {
            double[][] sequences = new double[][] 
            {
                new double[] { 0, 3, 1, 2 },
            };


            var hmm = HiddenMarkovModel.CreateGeneric(new Forward(4), 4);

            var teacher = new BaumWelchLearning<GeneralDiscreteDistribution>(hmm)
            {
                Tolerance = 1e-10,
                Iterations = 0
            };
            double ll = teacher.Run(sequences);

            double l11, l12, l13, l14;

            double p1 = hmm.Predict(new double[] { 0 }, out l11);
            double p2 = hmm.Predict(new double[] { 0, 3 }, out l12);
            double p3 = hmm.Predict(new double[] { 0, 3, 1 }, out l13);
            double p4 = hmm.Predict(new double[] { 0, 3, 1, 2 }, out l14);

            Assert.AreEqual(3, p1);
            Assert.AreEqual(1, p2);
            Assert.AreEqual(2, p3);
            Assert.AreEqual(2, p4);

            double l21 = hmm.Evaluate(new double[] { 0, 3 });
            double l22 = hmm.Evaluate(new double[] { 0, 3, 1 });
            double l23 = hmm.Evaluate(new double[] { 0, 3, 1, 2 });
            double l24 = hmm.Evaluate(new double[] { 0, 3, 1, 2, 2 });

            Assert.AreEqual(l11, l21);
            Assert.AreEqual(l12, l22);
            Assert.AreEqual(l13, l23);
            Assert.AreEqual(l14, l24, 1e-2);
            Assert.IsFalse(double.IsNaN(l14));
            Assert.IsFalse(double.IsNaN(l24));

            double ln1;
            double[] pn = hmm.Predict(new double[] { 0 }, 4, out ln1);

            Assert.AreEqual(4, pn.Length);
            Assert.AreEqual(3, pn[0]);
            Assert.AreEqual(1, pn[1]);
            Assert.AreEqual(2, pn[2]);
            Assert.AreEqual(2, pn[3]);

            double ln2 = hmm.Evaluate(new double[] { 0, 3, 1, 2, 2 });

            Assert.AreEqual(ln1, ln2, 1e-2);
            Assert.IsFalse(double.IsNaN(ln1));
            Assert.IsFalse(double.IsNaN(ln2));


            // Get the mixture distribution defining next state likelihoods
            Mixture<GeneralDiscreteDistribution> mixture = null;
            double ml11;
            double mp1 = hmm.Predict(new double[] { 0 }, out ml11, out mixture);

            Assert.AreEqual(l11, ml11);
            Assert.AreEqual(p1, mp1);
            Assert.IsNotNull(mixture);

            Assert.AreEqual(4, mixture.Coefficients.Length);
            Assert.AreEqual(4, mixture.Components.Length);
            Assert.AreEqual(0, mixture.Coefficients[0], 1e-10);
            Assert.AreEqual(1, mixture.Coefficients[1], 1e-10);
            Assert.AreEqual(0, mixture.Coefficients[2], 1e-10);
            Assert.AreEqual(0, mixture.Coefficients[3], 1e-10);

            for (int i = 0; i < mixture.Coefficients.Length; i++)
                Assert.IsFalse(double.IsNaN(mixture.Coefficients[i]));

        }

        [TestMethod()]
        public void PredictTest2()
        {
            // Create continuous sequences. In the sequence below, there
            // seems to be two states, one for values equal to 1 and another
            // for values equal to 2.
            double[][] sequences = new double[][] 
            {
                new double[] { 1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2,1,2 }             
            };

            // Specify a initial normal distribution for the samples.
            NormalDistribution density = new NormalDistribution();

            // Creates a continuous hidden Markov Model with two states organized in a forward
            //  topology and an underlying univariate Normal distribution as probability density.
            var model = new HiddenMarkovModel<NormalDistribution>(new Ergodic(2), density);

            // Configure the learning algorithms to train the sequence classifier until the
            // difference in the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<NormalDistribution>(model)
            {
                Tolerance = 0.0001,
                Iterations = 0,

                // However, we will need to specify a regularization constant as the
                //  variance of each state will likely be zero (all values are equal)
                FittingOptions = new NormalOptions() { Regularization = double.Epsilon }
            };

            // Fit the model
            double likelihood = teacher.Run(sequences);


            double a1 = model.Predict(new double[] { 1, 2, 1 });
            double a2 = model.Predict(new double[] { 1, 2, 1, 2 });

            Assert.AreEqual(2, a1, 1e-10);
            Assert.AreEqual(1, a2, 1e-10);
            Assert.IsFalse(Double.IsNaN(a1));
            Assert.IsFalse(Double.IsNaN(a2));

            double p1, p2;
            Mixture<NormalDistribution> d1, d2;
            double b1 = model.Predict(new double[] { 1, 2, 1 }, out p1, out d1);
            double b2 = model.Predict(new double[] { 1, 2, 1, 2 }, out p2, out d2);

            Assert.AreEqual(2, b1, 1e-10);
            Assert.AreEqual(1, b2, 1e-10);
            Assert.IsFalse(Double.IsNaN(b1));
            Assert.IsFalse(Double.IsNaN(b2));

            Assert.AreEqual(0, d1.Coefficients[0]);
            Assert.AreEqual(1, d1.Coefficients[1]);

            Assert.AreEqual(1, d2.Coefficients[0]);
            Assert.AreEqual(0, d2.Coefficients[1]);
        }

        [TestMethod()]
        public void PredictTest3()
        {
            // We will try to create a Hidden Markov Model which
            // can recognize (and predict) the following sequences:
            double[][] sequences = 
            {
                new double[] { 1, 2, 3, 4, 5 },
                new double[] { 1, 2, 4, 3, 5 },
                new double[] { 1, 2, 5 },
            };

            // Creates a new left-to-right (forward) Hidden Markov Model
            //  with 4 states for an output alphabet of six characters.
            var hmm = HiddenMarkovModel.CreateGeneric(new Forward(4), 6);

            // Try to fit the model to the data until the difference in
            //  the average log-likelihood changes only by as little as 0.0001
            var teacher = new BaumWelchLearning<GeneralDiscreteDistribution>(hmm)
            {
                Tolerance = 0.0001,
                Iterations = 0
            };

            // Run the learning algorithm on the model
            double logLikelihood = teacher.Run(sequences);

            // Now, we will try to predict the next
            //   observations after a base sequence

            double[] input = { 1, 2 }; // base sequence for prediction


            // Predict the next observation in sequence
            Mixture<GeneralDiscreteDistribution> mixture = null;

            double prediction = hmm.Predict(input, out mixture);


            // At this point, prediction probabilities
            // should be equilibrated around 3, 4 and 5
            Assert.AreEqual(4, mixture.Mean, 0.1);
            Assert.IsFalse(double.IsNaN(mixture.Mean));


            double[] input2 = { 1 };

            // The only possible value after 1 must be 2.
            prediction = hmm.Predict(input2, out mixture);

            Assert.AreEqual(2, prediction);
        }


    }
}
