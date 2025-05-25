using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace DDoS_IDS
{
    public class Neural_Network
    {
        public Neuron[] input_layer { get; private set; }
        public Neuron[] hidden_layer { get; private set; }
        public Neuron output_layer { get; private set; }

        public List<double> epoch_errors { get; private set; } = new List<double>();

        public int max_epochs { get; set; }
        public double max_error { get; set; }
        public double learning_rate { get; set; }


        public Neural_Network(int input_neurons_number, int hidden_neurons_number, bool Sigmoid_TanH, double learning_Rate)
        {
            input_layer = new Neuron[input_neurons_number];
            for (int i = 0; i < input_neurons_number; i++)
            {
                input_layer[i] = new Neuron(1, Sigmoid_TanH);
            }

            hidden_layer = new Neuron[hidden_neurons_number];
            for (int i = 0; i < hidden_neurons_number; i++)
            {
                hidden_layer[i] = new Neuron(input_neurons_number, Sigmoid_TanH);
            }


            output_layer = new Neuron(hidden_neurons_number, Sigmoid_TanH);

            learning_rate = learning_Rate;

        }

        //functia de forward propagation
        public double Forward_Propagation(double[] inputs, bool Sigmoid_TanH)
        {

            // nu aplicam functii la input
            for (int i = 0; i < input_layer.Length; i++)
            {
                input_layer[i].Output = inputs[i];
            }


            //input -> hidden, aplicam functii
            double[] hidden_Inputs = input_layer.Select(neuron => neuron.Output).ToArray();

            if (Sigmoid_TanH == true)
            {
                for (int i = 0; i < hidden_layer.Length; i++)
                {
                    hidden_layer[i].Inputs = hidden_Inputs;
                    hidden_layer[i].Output_Function(true);
                }
            }
            else
            {
                for (int i = 0; i < hidden_layer.Length; i++)
                {
                    hidden_layer[i].Inputs = hidden_Inputs;
                    hidden_layer[i].Output_Function(false);
                }
            }

            //hidden -> output, aplicam functii
            double[] output_Inputs = hidden_layer.Select(neuron => neuron.Output).ToArray();
            output_layer.Inputs = output_Inputs;


            if (Sigmoid_TanH == true)
            {
                return output_layer.Output_Function(true);
            }
            else
            {
                return output_layer.Output_Function(false);
            }
        }

        //functia de back propagation
        public void Back_Propagation(double expected_Output, bool Sigmoid_TanH)
        {
            //output
            double output_error = expected_Output - output_layer.Output;

            double output_derivate_error;

            if (Sigmoid_TanH) // derivata sig
            {
                output_derivate_error = output_error * output_layer.Output * (1 - output_layer.Output);
            }
            else // derivata tanh
            {
                output_derivate_error = output_error * (1 - Math.Pow(output_layer.Output, 2));
            }


            for (int i = 0; i < output_layer.Weights.Length; i++)   //schimbarea tariilor sinaptice
            {
                double output_weight_update = learning_rate * output_derivate_error * hidden_layer[i].Output;
                output_layer.Weights[i] = output_layer.Weights[i] + output_weight_update;
            }


            //hidden
            for (int i = 0; i < hidden_layer.Length; i++)
            {
                double hidden_error = output_derivate_error * output_layer.Weights[i];
                double hidden_derivate_error;

                if (Sigmoid_TanH) // derivata sigmoid
                {
                    hidden_derivate_error = hidden_error * hidden_layer[i].Output * (1 - hidden_layer[i].Output);
                }
                else // derivata tanh
                {
                    hidden_derivate_error = hidden_error * (1 - Math.Pow(hidden_layer[i].Output, 2));
                }

                for (int j = 0; j < hidden_layer[i].Weights.Length; j++)   //schimbarea tariilor sinaptice
                {
                    double hidden_weight_update = learning_rate * hidden_derivate_error * input_layer[j].Output;
                    hidden_layer[i].Weights[j] = hidden_layer[i].Weights[j] + hidden_weight_update;
                }
            }
        }


        //functia de antrenare (train)
        public void Train(double[][] inputs, double[] expected_Outputs, bool Sigmoid_TanH)
        {
            epoch_errors.Clear();

            for (int epoch = 0; epoch < max_epochs; epoch++)
            {
                double total_error = 0;

                for (int i = 0; i < inputs.Length; i++)
                {
                    double output = Forward_Propagation(inputs[i], Sigmoid_TanH);

                    double step_error = 0.5 * Math.Pow(expected_Outputs[i] - output, 2);  //MSE
                    total_error = total_error + step_error;

                    Back_Propagation(expected_Outputs[i], Sigmoid_TanH);
                }

                double epoch_error = total_error / inputs.Length; //eroarea per epoca
                epoch_errors.Add(epoch_error);


                if (epoch_error <= max_error)
                {
                    break;
                }
            }
        }


        //functia de salvare a modelului (tarii sinaptice)
        public void Save_Model(string filePath, Dictionary<int, (double min, double max)> norm_Min_Max_values)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(input_layer.Length);
                writer.WriteLine(hidden_layer.Length);          //salvam si nr neuroni de input, hidden, nr. de tarii pt output
                writer.WriteLine(output_layer.Weights.Length);

                foreach (var neuron in hidden_layer)
                {
                    foreach (var weight in neuron.Weights)
                    {
                        writer.WriteLine(weight);
                    }
                }

                foreach (var weight in output_layer.Weights)
                {
                    writer.WriteLine(weight);
                }

                  // salvare valorile minime si maxime pentru normalizare
                  writer.WriteLine("MINMAX");
                  foreach (var kvp in norm_Min_Max_values)
                  {
                      writer.WriteLine($"{kvp.Key},{kvp.Value.min},{kvp.Value.max}");
                  }
            }
        }


        //functie incarcare model (tarii) ATENTIE: trebuie sa avem acelasi numar de straturi
    public bool Load_Model(string filePath, out Dictionary<int, (double min, double max)> norm_Min_Max_values)
     {
     norm_Min_Max_values = new Dictionary<int, (double min, double max)>();

     using (StreamReader reader = new StreamReader(filePath))
     {
         int input_Neuron_Count = int.Parse(reader.ReadLine());
         int hidden_Neuron_Count = int.Parse(reader.ReadLine());
         int output_Weight_Count = int.Parse(reader.ReadLine());

         if (Input_Layer.Length != input_Neuron_Count || Hidden_Layer.Length != hidden_Neuron_Count)
         {
             MessageBox.Show($"Error! Expected hidden neurons: {Hidden_Layer.Length} | found: {hidden_Neuron_Count}");
             return false;
         }

         foreach (var neuron in Hidden_Layer)
         {
             for (int i = 0; i < neuron.Weights.Length; i++)
             {
                 neuron.Weights[i] = double.Parse(reader.ReadLine());
             }
         }

         for (int i = 0; i < Output_Layer.Weights.Length; i++)
         {
             Output_Layer.Weights[i] = double.Parse(reader.ReadLine());
         }

         string line = reader.ReadLine();
         if (line == "MINMAX")
         {
             while (!reader.EndOfStream)
             {
                 string[] parts = reader.ReadLine().Split(',');
                 int col = int.Parse(parts[0]);
                 double min = double.Parse(parts[1]);
                 double max = double.Parse(parts[2]);

                 norm_Min_Max_values[col] = (min, max);
             }
         }
     }

     MessageBox.Show("Model loaded successfully.");
     return true;
 }


    }

}
