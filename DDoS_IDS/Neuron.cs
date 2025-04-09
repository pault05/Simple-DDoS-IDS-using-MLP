using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DDoS_IDS
{
    public class Neuron
    {
        public double[] Inputs { get; set; }
        public double[] Weights { get; set; }
        public double Output { get; set; }

        private Random Rand;  // pt weight

        public Neuron(int input_Count, bool Sigmoid_or_TanH)
        {
            Weights = new double[input_Count];
            Rand = new Random();
            Initialize_Weights();

        }

        //weight random de inceput
        private void Initialize_Weights()
        {
            for (int i = 0; i < Weights.Length; i++)
            {
                Weights[i] = (Rand.NextDouble() * 2) - 1;    // - 0.5 pt [-0.5 , 0.5]
              
            }
        }

        //functia sigmoid
        public double Sigmoid(double sum)
        {
            return 1.0 / (1.0 + Math.Exp(-sum));
        }

        //functia tangenta hiperbolica
        public double TanH(double sum)
        {
            return (Math.Exp(sum) - Math.Exp(-sum)) / (Math.Exp(sum) + Math.Exp(-sum));
        }

        //functia de output, depinde de radio_button
        public double Output_Function(bool Sigmoid_TanH)
        {
            double sum = 0.0;
            for (int i = 0; i < Weights.Length; i++)
            {
                sum = sum + Weights[i] * Inputs[i];
            }

            if (Sigmoid_TanH)
            {
                Output = Sigmoid(sum);
            }
            else
            {
                Output = TanH(sum);
            }



            return Output;
        }
    }
}
