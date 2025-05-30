﻿using OfficeOpenXml;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using System.Text;
using OxyPlot.Annotations;

namespace DDoS_IDS
{
    public partial class Main_Form : Form
    {
        DataTable dt;
        DataTable dt_real;
        double[,] dataSet_train;
        double[,] dataSet_test;
        double[,] dataSet_real_test;
        int train_Rows, test_Rows;

        public List<double> epoch_errors { get; private set; } = new List<double>();
        private List<double> test_Results = new List<double>(); //testare retea neuronala
        private List<double> real_test_Results = new List<double>(); // testare retea neuronala pe date reale

        private Dictionary<int, (double min, double max)> norm_Min_Max_values = new Dictionary<int, (double min, double max)>(); // pentru persistenta 

        Neural_Network network;   //reteaua in sine (obiect)
        int hidden_Layer_Neurons = 15;
        int max_Epochs = 5000;             // nu sunt folosite
        double max_Error = 0.01;
        double learning_Rate = 0.05;

        public Main_Form()
        {
            InitializeComponent();

            dt = new DataTable();

            dataSet_train = new double[0, 0];

            radioButton_sigmoid.Checked = true;

            dataGridView_data.EnableHeadersVisualStyles = false;
        }


        // functie incarcare fisier de tip excel in aplicatie (train + test)
        private void Load_Excel_File(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage package = new ExcelPackage(new FileInfo(filePath));
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

            int total_Rows = worksheet.Dimension.End.Row - 1;
            int cols = worksheet.Dimension.End.Column;

            train_Rows = (int)(total_Rows * 0.7);    //procentul de train-test
            test_Rows = total_Rows - train_Rows;

            dataSet_train = new double[train_Rows, cols];
            dataSet_test = new double[test_Rows, cols];

            dt.Columns.Clear();
            for (int i = 1; i <= cols; i++)
            {
                dt.Columns.Add(worksheet.Cells[1, i].Value?.ToString() ?? $"Column {i}");
            }

            int rowIndex = 0;
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++, rowIndex++)
            {
                DataRow newRow = dt.NewRow();
                for (int col = 1; col <= cols; col++)
                {
                    double value = Convert.ToDouble(worksheet.Cells[row, col].Value ?? "0");
                    newRow[col - 1] = value;

                    if (rowIndex < train_Rows)
                        dataSet_train[rowIndex, col - 1] = value;     // impartirea train-test
                    else
                        dataSet_test[rowIndex - train_Rows, col - 1] = value;
                }
                dt.Rows.Add(newRow);
            }

            dataGridView_data.DataSource = dt;
          //  Randomize_Rows();  //doar randuri antrenare
            Color_Number_Rows();
        }

        //functie colorare train-test, numerotare randuri test
        private void Color_Number_Rows()
        {
            if (!dataGridView_data.Columns.Contains("RowNumber"))
            {
                dataGridView_data.Columns.Insert(0, new DataGridViewTextBoxColumn
                {
                    Name = "RowNumber",
                    HeaderText = "Row #",
                    ReadOnly = true
                });
            }

            int test_Row_index = 1;
            int label_Column_Index = dataGridView_data.ColumnCount - 1;

            for (int i = 0; i < dataGridView_data.Rows.Count; i++)
            {
                if (i < train_Rows)
                {
                    dataGridView_data.Rows[i].DefaultCellStyle.BackColor = Color.Yellow;     //galben == train 
                    dataGridView_data.Rows[i].Cells["RowNumber"].Value = null;
                }
                else
                {
                    dataGridView_data.Rows[i].DefaultCellStyle.BackColor = Color.LightGray;    //gri == test; + numerotare

                    dataGridView_data.Rows[i].Cells["RowNumber"].Value = test_Row_index;
                    test_Row_index++;
                }
            }
            dataGridView_data.Refresh();
        }


        //functie de randomizare date antrenare (train)
        private void Randomize_Rows()
        {
            Random rnd = new Random();
            int rows = dataSet_train.GetLength(0);
            int cols = dataSet_train.GetLength(1);

            for (int i = rows - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);

                for (int col = 0; col < cols; col++)
                {
                    double temp = dataSet_train[i, col];
                    dataSet_train[i, col] = dataSet_train[j, col];
                    dataSet_train[j, col] = temp;
                }

                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    var tempValue = dt.Rows[i][col];
                    dt.Rows[i][col] = dt.Rows[j][col];
                    dt.Rows[j][col] = tempValue;
                }
            }

            dataGridView_data.Refresh();
        }


        //butonul de incarcare fisier excel (train + test)
        private void button_load_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                Load_Excel_File(filePath);
            }
        }


        //functie de normalizare date de ANTRENARE, in functie de radio_button
        private void Data_Normalization_Train()
 {
     int rows = dataSet_train.GetLength(0);
     int cols = dataSet_train.GetLength(1);

     norm_Min_Max_values.Clear();  // Clear any old values

     for (int col = 0; col < cols; col++)
     {
         double min = double.MaxValue;
         double max = double.MinValue;

         for (int row = 0; row < rows; row++)
         {
             double value = dataSet_train[row, col];
             if (value < min) min = value;
             if (value > max) max = value;
         }

         norm_Min_Max_values[col] = (min, max);
     }

     for (int row = 0; row < rows; row++)
     {
         for (int col = 0; col < cols; col++)
         {
             double value = dataSet_train[row, col];
             double min = norm_Min_Max_values[col].min;
             double max = norm_Min_Max_values[col].max;

             if (max != min)
             {
                 double normalizedValue = radioButton_sigmoid.Checked
                     ? (value - min) / (max - min)
                     : 2 * ((value - min) / (max - min)) - 1;

                 dataSet_train[row, col] = normalizedValue;
                 dt.Rows[row][col] = normalizedValue.ToString("F6");
             }
         }
     }

     if (dataGridView_data.InvokeRequired)
     {
         dataGridView_data.Invoke(new Action(() => dataGridView_data.Refresh()));
     }
     else
     {
         dataGridView_data.Refresh();
     }
 }


        //buton normalizare
        private void button_normalize_Click_1(object sender, EventArgs e)
        {
            Thread th = new Thread(() =>
            {
                if (dt.Rows.Count != 0)
                {
                    Data_Normalization_Train();
                }

            })
            { IsBackground = true };
            th.Start();
        }


        //functia de initilizare retea neuronala; parametrii sunt preluati din text_box - uri
        private void Initialize_Network()
        {

            if (!int.TryParse(textBox_hl_neurons.Text, out hidden_Layer_Neurons))
            {
                MessageBox.Show("Please give a number for the hidden neurons!");
                return;
            }

            if (!int.TryParse(textBox_max_epochs.Text, out max_Epochs))
            {
                MessageBox.Show("Please give a number for the maximum epochs!");
                return;
            }

            if (!double.TryParse(textBox_max_error.Text, out max_Error))
            {
                MessageBox.Show("Please give a number for the maximum error!");
                return;
            }

            if (!double.TryParse(textBox_learning_rate.Text, out learning_Rate))
            {
                MessageBox.Show("Please give a number for the learning rate!");
                return;
            }

            bool Sigmod_TanH = radioButton_sigmoid.Checked;


            //numar FIX de neuroni INPUT 
            network = new Neural_Network(10, hidden_Layer_Neurons, Sigmod_TanH, learning_Rate)
            {
                max_epochs = max_Epochs,
                max_error = max_Error

            };
        }


        //butonul de antrenare a retelei. ATENTIE: trebuie sa avem parametrii, fisier excel, normalizare ...
        private void button_train_Click(object sender, EventArgs e)
        {
             Initialize_Network();

            ////  valori fixe(pt usurinta testarii)
            //network = new Neural_Network(10, 15, radioButton_sigmoid.Checked, 0.005)
            //{
            //    //max_epochs = max_Epochs,
            //    //max_error = max_Error
            //    max_epochs = 300,
            //    max_error = 0.001
            //};

            Thread thread = new Thread(() =>
            {

                int rows = dataSet_train.GetLength(0);
                int cols = dataSet_train.GetLength(1) - 1;  // fara label

                double[][] inputs = new double[rows][];
                double[] expected_outputs = new double[rows];

                for (int i = 0; i < rows; i++)
                {
                    inputs[i] = new double[cols];
                    for (int j = 0; j < cols; j++)
                    {
                        inputs[i][j] = dataSet_train[i, j];
                    }
                    expected_outputs[i] = dataSet_train[i, cols];
                }

                network.Train(inputs, expected_outputs, radioButton_sigmoid.Checked);


                Plot_Training();
            })
            { IsBackground = true };
            thread.Start();

        }

        //functie de trasare (plot) a procesului de antrenare -> OxyPlot library (nuGet)
        private void Plot_Training()
        {
            var plotModel = new PlotModel { Title = "Training error per epoch" };

            var lineSeries = new LineSeries
            {
                Title = "Error",
                Color = OxyColors.Blue,
                MarkerType = MarkerType.Circle,
                MarkerSize = 3,
                MarkerStroke = OxyColors.Red
            };

            for (int i = 0; i < network.epoch_errors.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i + 1, network.epoch_errors[i]));
            }

            plotModel.Series.Add(lineSeries);

            var xAxis = new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Title = "Epoch",
                Minimum = 1,
                Maximum = network.epoch_errors.Count
            };
            plotModel.Axes.Add(xAxis);

            var yAxis = new LinearAxis
            {
                Position = AxisPosition.Left,
                Title = "Error",
                Minimum = 0,
                Maximum = network.epoch_errors.Max()
            };
            plotModel.Axes.Add(yAxis);


            plotView_training.Model = plotModel;
            plotView_training.InvalidatePlot(true);
        }


        //buton de testare, doar apelam functia de testare
        private void button_test_Click(object sender, EventArgs e)
        {
            Test_Data();
        }


        //functie de trasare (plot) a procesului de testare -> OxyPlot library (nuGet) -> spikes, bursts, etc.
        //putem vedea daca atacul a fost liniar sau impartit pe blocuri
        private void Plot_Testing()
        {
            var plot_Model_Graph = new PlotModel { Title = "Test results" };

            var lineSeries = new LineSeries
            {
                Title = "Test Predictions",
                Color = OxyColors.Blue,
                MarkerType = MarkerType.Circle,
                MarkerSize = 2,
                MarkerStroke = OxyColors.Black
            };

            for (int i = 0; i < test_Results.Count; i++)
            {
                lineSeries.Points.Add(new DataPoint(i + 1, test_Results[i]));
            }

            plot_Model_Graph.Series.Add(lineSeries);

            plot_Model_Graph.Axes.Add(new LinearAxis { Position = AxisPosition.Bottom, Title = "Rows" });

            plot_Model_Graph.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Prediction" });

            plotView_testing.Model = plot_Model_Graph;
            plotView_testing.InvalidatePlot(true);
        }


        // functie de trasare (plot) a matricei de confuzie -> OxyPlot library (nuGet) -> TP, TN, FP, FN
        private void Plot_Confusion_Matrix(int true_Positive, int true_Negative, int false_Positive, int false_Negative)
        {
            var plot_Model_Matrix = new PlotModel { Title = "Confusion matrix" };

            var color_axis = new LinearColorAxis
            {
                Position = AxisPosition.Right,
                Palette = new OxyPalette(
                    OxyColors.YellowGreen,  // TP + TN (corecte)
                    OxyColors.Red     // FP + FN (incorecte)
                ),
                IsAxisVisible = false, 
                Minimum = 0,
                Maximum = 1
            };

            plot_Model_Matrix.Axes.Add(color_axis);

            // x
            var xAxis = new CategoryAxis { Position = AxisPosition.Bottom, Title = "Actual Label" };
            xAxis.Labels.AddRange(new List<string> { "Negative", "Positive" });

            // y 
            var yAxis = new CategoryAxis { Position = AxisPosition.Left, Title = "Predicted Label" };
            yAxis.Labels.AddRange(new List<string> { "Negative", "Positive" });

            plot_Model_Matrix.Axes.Add(xAxis);

            plot_Model_Matrix.Axes.Add(yAxis);

            // categoriile matricei de confuzie
            var map = new HeatMapSeries
            {
                X0 = 0,
                X1 = 2,
                Y0 = 0,
                Y1 = 2,
                Data = new double[,]
                {
            { 0, 1 }, // FN (rosu)  / TN (galben)
            { 1, 0 }  // FP (rosu) / TP (galben)
                },
                Interpolate = false
            };
            plot_Model_Matrix.Series.Add(map);

            // labeluri pt matricea de confuzie
            var labels = new List<TextAnnotation>     // tip de label din oxyplot
            {
                new TextAnnotation {
                    Text = $"{false_Positive}",
                    TextPosition = new DataPoint(0, 2),
                    Stroke = OxyColors.Black,
                    FontSize = 20,
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Middle
                }, 

                new TextAnnotation {
                    Text = $"{true_Positive}",
                    TextPosition = new DataPoint(1.5, 2.0),
                    Stroke = OxyColors.Black,
                    FontSize = 20,
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Middle
                },

                new TextAnnotation {
                    Text = $"{true_Negative}",
                    TextPosition = new DataPoint(0, 0.1),
                    Stroke = OxyColors.Black,
                    FontSize = 20,
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Middle
                },

                new TextAnnotation {
                    Text = $"{false_Negative}",
                    TextPosition = new DataPoint(1.5, 0.1),
                    Stroke = OxyColors.Black,
                    FontSize = 20,
                    TextHorizontalAlignment = OxyPlot.HorizontalAlignment.Center,
                    TextVerticalAlignment = OxyPlot.VerticalAlignment.Middle
                }
            };


            foreach (var label in labels)
                plot_Model_Matrix.Annotations.Add(label);

            // refresh, aplicare plot
            plotView_matrix.Model = plot_Model_Matrix;
            plotView_matrix.InvalidatePlot(true);
        }



        //functia de normalizare date de TESTARE, in functie de radio_button
        private void Data_Normalization_Test()
        {
            int rows = dataSet_test.GetLength(0);
            int cols = dataSet_test.GetLength(1);

            Dictionary<int, (double min, double max)> min_Max_Values = new Dictionary<int, (double min, double max)>();

            for (int col = 0; col < cols; col++)
            {
                double min = double.MaxValue;
                double max = double.MinValue;

                for (int row = 0; row < dataSet_test.GetLength(0); row++)
                {
                    double value = dataSet_test[row, col];
                    if (value < min) min = value;
                    if (value > max) max = value;
                }

                min_Max_Values[col] = (min, max);
            }

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    double value = dataSet_test[row, col];
                    double min = min_Max_Values[col].min;
                    double max = min_Max_Values[col].max;

                    if (max != min)
                    {
                        double normalized_Value;
                        if (radioButton_sigmoid.Checked)
                        {
                            normalized_Value = (value - min) / (max - min);
                        }
                        else
                        {
                            normalized_Value = 2 * ((value - min) / (max - min)) - 1;
                        }
                        dataSet_test[row, col] = normalized_Value;
                    }
                }
            }
        }


        //buton salvare tarii sinaptice (model) in fisier .txt 
        private void button_save_model_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Save Model"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                network.Save_Model(saveFileDialog.FileName, norm_Min_Max_values);
                MessageBox.Show("Succes!");
            }
        }

        //buton incarcare model din fisier .txt ; ATENTIE: trebuie sa aiba exact numarul de neuroni de intrare
        private void button_load_model_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files|*.txt",
                Title = "Load Model"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Initialize_Network();

                   if (network.Load_Model("model.txt", out var loadedMinMax))
                    {
                        norm_Min_Max_values = loadedMinMax;
                    } 
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading model: {ex.Message}");
                }
            }
        }


        // functie de segmentare blocuri; necesara pt spike detection
        private List<double[]> Segment_Data(int block_Size)
        {
        List<double[]> data_blocks = new List<double[]>();

        for (int i = 0; i < test_Results.Count; i += block_Size)
            {
        int current_block_size = Math.Min(block_Size, test_Results.Count - i);
        double[] block = new double[current_block_size];

        for (int j = 0; j < current_block_size; j++)
            {
            int predicted_Label = test_Results[i + j] >= (radioButton_sigmoid.Checked ? 0.5 : 0.0) ? 1 : 0;
            block[j] = predicted_Label;
            }

            data_blocks.Add(block);
            }

            return data_blocks;
        }


        public List<double[]> Segment_Data_Real_Test(int block_Size)
        {
            List<double[]> data_blocks_real = new List<double[]>();

            for (int i = 0; i < real_test_Results.Count; i += block_Size)
        {
        int current_block_size = Math.Min(block_Size, real_test_Results.Count - i);
        double[] block = new double[current_block_size];

        for (int j = 0; j < current_block_size; j++)
        {
            int predicted_Label = real_test_Results[i + j] >= (radioButton_sigmoid.Checked ? 0.5 : 0.0) ? 1 : 0;
            block[j] = predicted_Label;
        }

        data_blocks_real.Add(block);
        }

        return data_blocks_real;
        }


        //functie de analiza a anomaliilor din datele testate / spike detection
private void Spike_Analysis(int block_Size, double spike_Threshold, int consecutive_Spike_Limit, double spike_ratio_Threshold = 0.5)
{

    // verificare date de intrare
    var data_Blocks = Segment_Data(block_Size);
    int total_Blocks = data_Blocks.Count;
    int spike_Blocks = 0;
    int consecutive_Spikes = 0;
    bool sustained_Attack_Detected = false;

    // timestamp pentru logging
    string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    string log_FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Advanced_Spike_Analysis_{timestamp}.txt");

    using (StreamWriter log = new StreamWriter(log_FilePath))
    {
        log.WriteLine($"[Advanced Spike Detection Log - {DateTime.Now}]");
        log.WriteLine($"Block size: {block_Size} \n Spike threshold: {spike_Threshold} \n Consecutive limit: {consecutive_Spike_Limit} \n Overall ratio threshold: {spike_ratio_Threshold}");
        log.WriteLine("------------------------------------------------------");

        // analiza blocurilor de date
        for (int i = 0; i < total_Blocks; i++)
        {
            double[] block = data_Blocks[i];
            int ddos_Count = block.Count(label => label == 1);
            double ddos_Ratio = (double)ddos_Count / block_Size;

            string status;
            if (ddos_Ratio > spike_Threshold)
            {
                spike_Blocks++;

                consecutive_Spikes++;

                status = $"Spike - {ddos_Count}/{block_Size} = {ddos_Ratio:F2}";
                if (consecutive_Spikes >= consecutive_Spike_Limit)
                {
                    sustained_Attack_Detected = true;
                    log.WriteLine($"[Block {i + 1}] {status} → Sustained Attack Detected! (Consecutive spikes = {consecutive_Spikes})");

                    break;
                }
            }
            else
            {
                status = $"Normal - {ddos_Count}/{block_Size} = {ddos_Ratio:F2}";
                consecutive_Spikes = 0;
            }

            log.WriteLine($"[Block {i + 1}] {status}");
        }

        double spike_Ratio = (double)spike_Blocks / total_Blocks;
        double severity_Score = spike_Ratio * 100;   //procent pentru severitatea testului

        //caracteristici de logging
        log.WriteLine("------------------------------------------------------");
        log.WriteLine($"Total Blocks: {total_Blocks}");
        log.WriteLine($"Spike Blocks: {spike_Blocks}");
        log.WriteLine($"Spike Ratio: {spike_Ratio:F2}");
        log.WriteLine($"Severity Score: {severity_Score:F1}/100");

        if (!sustained_Attack_Detected && spike_Ratio >= spike_ratio_Threshold)
        {
            log.WriteLine("[RESULT] Frequent spikes detected across dataset. DDoS is highly probable.");

            sustained_Attack_Detected = true;
        }
        else if (!sustained_Attack_Detected && spike_Blocks > 0)
        {
            log.WriteLine("[RESULT] Some spikes detected, but not frequent or sustained enough.");
        }
        else if (spike_Blocks == 0)
        {
            log.WriteLine("[RESULT] No spikes detected.");
        }
    }

    // feedback in UI
    if (sustained_Attack_Detected)
    {
        MessageBox.Show("DDoS attack detected! See detailed analysis in log file.", "Spike Detection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
    else if (spike_Blocks > 0)
    {
        MessageBox.Show("Some anomalies detected. DDoS not confirmed. See log for details.", "Spike Detection", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    else
    {
        MessageBox.Show("No anomalies or spikes detected.", "Spike Detection", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
      

        //functia de testare retea neuronala, dupa antrenare
         private void Test_Data()
 {
     try
     {
         Thread th = new Thread(() =>
         {
             if (dataSet_test == null || dataSet_test.Length == 0)
             {
                 MessageBox.Show("Please load a dataset and train the network before testing.");
                 return;
             }

             Data_Normalization_Test();

             test_Results.Clear();
             List<int> actual_Labels = new List<int>();
             List<int> predicted_Labels = new List<int>();

             int false_Positives = 0;
             int false_Negatives = 0;
             int true_Positives = 0;
             int true_Negatives = 0;

             int correct_Predictions = 0;

             double threshold = radioButton_sigmoid.Checked ? 0.5 : 0.0;
             int test_offset_row = dataGridView_data.Rows.Count - dataSet_test.GetLength(0);

             test_Results.Clear();
             actual_Labels.Clear();
             predicted_Labels.Clear();

             for (int i = 0; i < dataSet_test.GetLength(0); i++)
             {
                 double[] test_Input = new double[dataSet_test.GetLength(1) - 1];
                 for (int j = 0; j < dataSet_test.GetLength(1) - 1; j++)
                 {
                     test_Input[j] = dataSet_test[i, j];
                 }

                 double actual_Label = dataSet_test[i, dataSet_test.GetLength(1) - 1];
                 double prediction = network.Forward_Propagation(test_Input, radioButton_sigmoid.Checked);

                 test_Results.Add(prediction);

                 int predicted_Label = prediction >= threshold ? 1 : 0;

                 actual_Labels.Add((int)actual_Label);
                 predicted_Labels.Add(predicted_Label);

                 if (predicted_Label == 1 && actual_Label == 1)
                 {
                     true_Positives++;
                     correct_Predictions++;
                 }
                 else if (predicted_Label == 0 && actual_Label == 0)
                 {
                     true_Negatives++;
                     correct_Predictions++;
                 }
                 else if (predicted_Label == 1 && actual_Label == 0)
                 {
                     false_Positives++;
                     dataGridView_data.Rows[test_offset_row + i].DefaultCellStyle.BackColor = Color.OrangeRed;
                 }
                 else if (predicted_Label == 0 && actual_Label == 1)
                 {
                     false_Negatives++;
                     dataGridView_data.Rows[test_offset_row + i].DefaultCellStyle.BackColor = Color.DarkBlue;
                 }
             }

             int total_Predictions = dataSet_test.GetLength(0);

             double accuracy = (double)correct_Predictions / total_Predictions * 100;

             double precision = true_Positives + false_Positives > 0 ? (double)true_Positives / (true_Positives + false_Positives) : 0;

             double recall = true_Positives + false_Negatives > 0 ? (double)true_Positives / (true_Positives + false_Negatives) : 0;

             double f1 = precision + recall > 0 ? 2 * (precision * recall) / (precision + recall) : 0;


             // setari pentru detectie spike, atac continuu, anomalii
             int block_Size = 25;
             double spike_Threshold = 0.5;
             int consecutive_Spike_Limit = 5;

             Spike_Analysis(block_Size, spike_Threshold, consecutive_Spike_Limit, 0.5);

             dataGridView_data.Refresh();
             
             MessageBox.Show(
                 $"Testing complete:\n" +
                 $"False Positives: {false_Positives}\n" +
                 $"False Negatives: {false_Negatives}\n" +
                 $"Accuracy: {accuracy:F2}%\n" +
                 $"Precision: {precision:P2}\n" +
                 $"Recall: {recall:P2}\n" +
                 $"F1 Score: {f1:P2}",
                 "Evaluation Results",
                 MessageBoxButtons.OK,
                 MessageBoxIcon.Information);

             Plot_Testing(); //afisare predictii in grafic
             Plot_Confusion_Matrix(true_Positives, true_Negatives, false_Positives, false_Negatives); //afisare matrice de confuzie

         })
         { IsBackground = true };
         th.Start();
     }
     catch (Exception ex)
     {
         MessageBox.Show(ex.Message);
     }
 }


        // urmatoarele functii sunt destinate exclusiv testarii datelor reale, fara 'label'
        // desi sunt similare cu cele de mai sus, sunt separate pentru claritate si pentru a asigura amestecarea datelor

        //testarea pentru date reale, fara label
        private void button_real_test_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Excel Files|*.xlsx";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = openFileDialog.FileName;

                        // incarcare date reale
                        LoadExcelData_for_Real_Testing(filePath);

                        // normalizare date reale
                        Data_Normalization_for_Real_Testing();

                        // predict (forward prop., labels)
                        Testing_for_Real_Data(dataSet_real_test, dt_real);

                        // spike analysis pt date reale
                        Spike_Analysis_Real_Data(25, 0.5, 10, 0.5);

                        // salvare rezultate in tabela excel
                        Save_real_DataTable_to_Excel(dt_real, filePath.Replace(".xlsx", "_classified.xlsx"));

                        MessageBox.Show("Real data testing complete. Results saved to new Excel file.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }


        private void Spike_Analysis_Real_Data(int block_Size, double spike_Threshold, int consecutive_Spike_Limit, double spike_ratio_Threshold = 0.5)
    {

    // verificare date de intrare
    var data_Blocks = Segment_Data_Real_Test(block_Size);
    int total_Blocks = data_Blocks.Count;
    int spike_Blocks = 0;
    int consecutive_Spikes = 0;
    bool sustained_Attack_Detected = false;

    // timestamp pentru logging
    string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

    string log_FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"Real_Spike_Detection_Log_{timestamp}.txt");

    using (StreamWriter log = new StreamWriter(log_FilePath))
    {
        log.WriteLine($"[Real Spike Detection Log - {DateTime.Now}]");
        log.WriteLine($"Block size: {block_Size} \n Spike threshold: {spike_Threshold} \n Consecutive limit: {consecutive_Spike_Limit} \n Overall ratio threshold: {spike_ratio_Threshold}");
        log.WriteLine("------------------------------------------------------");

        // analiza blocurilor de date
        for (int i = 0; i < total_Blocks; i++)
        {
            double[] block = data_Blocks[i];
            int ddos_Count = block.Count(label => label == 1);
            double ddos_Ratio = (double)ddos_Count / block_Size;

            string status;
            if (ddos_Ratio > spike_Threshold)
            {
                spike_Blocks++;

                consecutive_Spikes++;

                status = $"Spike - {ddos_Count}/{block_Size} = {ddos_Ratio:F2}";
                if (consecutive_Spikes >= consecutive_Spike_Limit)
                {
                    sustained_Attack_Detected = true;
                    log.WriteLine($"[Block {i + 1}] {status} → Sustained Attack Detected! (Consecutive spikes = {consecutive_Spikes})");

                    break;
                }
            }
            else
            {
                status = $"Normal - {ddos_Count}/{block_Size} = {ddos_Ratio:F2}";
                consecutive_Spikes = 0;
            }

            log.WriteLine($"[Block {i + 1}] {status}");
        }

        double spike_Ratio = (double)spike_Blocks / total_Blocks;
        double severity_Score = spike_Ratio * 100;   //procent pentru severitatea testului

        //caracteristici de logging
        log.WriteLine("------------------------------------------------------");
        log.WriteLine($"Total Blocks: {total_Blocks}");
        log.WriteLine($"Spike Blocks: {spike_Blocks}");
        log.WriteLine($"Spike Ratio: {spike_Ratio:F2}");
        log.WriteLine($"Severity Score: {severity_Score:F1}/100");

        if (!sustained_Attack_Detected && spike_Ratio >= spike_ratio_Threshold)
        {
            log.WriteLine("[RESULT] Frequent spikes detected across dataset. DDoS is highly probable.");

            sustained_Attack_Detected = true;
        }
        else if (!sustained_Attack_Detected && spike_Blocks > 0)
        {
            log.WriteLine("[RESULT] Some spikes detected, but not frequent or sustained enough.");
        }
        else if (spike_Blocks == 0)
        {
            log.WriteLine("[RESULT] No spikes detected.");
        }
    }

    // feedback in UI
    if (sustained_Attack_Detected)
    {
        MessageBox.Show("DDoS attack detected! See detailed analysis in log file.", "Spike Detection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
    else if (spike_Blocks > 0)
    {
        MessageBox.Show("Some anomalies detected. DDoS not confirmed. See log for details.", "Spike Detection", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    else
    {
        MessageBox.Show("No anomalies or spikes detected.", "Spike Detection", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}


        //functia pentru incarcarea datelor reale
        private void LoadExcelData_for_Real_Testing(string filePath)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage package = new ExcelPackage(new FileInfo(filePath));
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

            int totalRows = worksheet.Dimension.End.Row - 1;
            int cols = worksheet.Dimension.End.Column;

            dataSet_real_test = new double[totalRows, cols];
            dt_real = new DataTable();

            for (int i = 1; i <= cols; i++)
            {
                dt_real.Columns.Add(worksheet.Cells[1, i].Value?.ToString() ?? $"Column {i}");
            }

            int rowIndex = 0;
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++, rowIndex++)
            {
                DataRow newRow = dt_real.NewRow();
                for (int col = 1; col <= cols; col++)
                {
                    double value = Convert.ToDouble(worksheet.Cells[row, col].Value ?? "0");
                    newRow[col - 1] = value;
                    dataSet_real_test[rowIndex, col - 1] = value;
                }
                dt_real.Rows.Add(newRow);
            }

            dataGridView_data.DataSource = dt_real;
        }


        //functia de normalizare a datelor reale 
    private void Data_Normalization_for_Real_Testing()
     {
      if (norm_Min_Max_values == null || norm_Min_Max_values.Count == 0)
      {
          MessageBox.Show("Min-Max normalization values are missing. Train or load a model first.");
          return;
      }

      int rows = dataSet_real_test.GetLength(0);
      int cols = dataSet_real_test.GetLength(1);

      for (int row = 0; row < rows; row++)
      {
          for (int col = 0; col < cols; col++)
          {
              if (!norm_Min_Max_values.ContainsKey(col)) continue;

              double value = dataSet_real_test[row, col];
              double min = norm_Min_Max_values[col].min;
              double max = norm_Min_Max_values[col].max;

              if (max != min)
              {
                  double normalized_Value = radioButton_sigmoid.Checked
                      ? (value - min) / (max - min)
                      : 2 * ((value - min) / (max - min)) - 1;

                  dataSet_real_test[row, col] = normalized_Value;
              }
          }
      }
  }

        //functia de testare, forward propagation, a datelor reale
    private void Testing_for_Real_Data(double[,] dataSet, DataTable dataTable)
 {
     // coloana Prediction trebuie sa existe in dataTable
     if (!dataTable.Columns.Contains("Prediction"))
         dataTable.Columns.Add("Prediction", typeof(int));

     real_test_Results.Clear();
     double threshold = radioButton_sigmoid.Checked ? 0.5 : 0.0;

     int row_Count = dataSet.GetLength(0);
     for (int row = 0; row < row_Count; row++)
     {
         double[] input_Row = new double[dataSet.GetLength(1)];
         for (int col = 0; col < dataSet.GetLength(1); col++)
         {
             input_Row[col] = dataSet[row, col];
         }

         double prediction = network.Forward_Propagation(input_Row, radioButton_sigmoid.Checked);
         int predicted_Label = prediction >= threshold ? 1 : 0;
         real_test_Results.Add(prediction); //spike analysis

         dataTable.Rows[row]["Prediction"] = predicted_Label;
     }
 }


        //functia de salvare a datelor obtinute in urma testarii datelor reale
        //afisam si in data_table, in aplicatie
        private void Save_real_DataTable_to_Excel(DataTable dt, string filePath)
        {
            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Results");
                worksheet.Cell(1, 1).InsertTable(dt);
                workbook.SaveAs(filePath);
            }
        }


    }
}
