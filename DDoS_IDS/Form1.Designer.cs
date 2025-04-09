namespace DDoS_IDS
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            dataGridView_data = new DataGridView();
            button_load_train_file = new Button();
            button_normalize = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            radioButton_sigmoid = new RadioButton();
            radioButton_tanh = new RadioButton();
            button_train = new Button();
            panel1 = new Panel();
            textBox_max_error = new TextBox();
            label4 = new Label();
            textBox_learning_rate = new TextBox();
            label3 = new Label();
            textBox_max_epochs = new TextBox();
            label2 = new Label();
            textBox_hl_neurons = new TextBox();
            label1 = new Label();
            button_test = new Button();
            plotView_testing = new OxyPlot.WindowsForms.PlotView();
            button_load_model = new Button();
            button_save_model = new Button();
            button_real_test = new Button();
            plotView_training = new OxyPlot.WindowsForms.PlotView();
            plotView_matrix = new OxyPlot.WindowsForms.PlotView();
            ((System.ComponentModel.ISupportInitialize)dataGridView_data).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView_data
            // 
            dataGridView_data.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_data.Location = new Point(3, 75);
            dataGridView_data.Name = "dataGridView_data";
            dataGridView_data.RowHeadersWidth = 51;
            dataGridView_data.Size = new Size(906, 393);
            dataGridView_data.TabIndex = 0;
            // 
            // button_load_train_file
            // 
            button_load_train_file.AutoSize = true;
            button_load_train_file.Location = new Point(21, 12);
            button_load_train_file.Name = "button_load_train_file";
            button_load_train_file.Size = new Size(136, 34);
            button_load_train_file.TabIndex = 1;
            button_load_train_file.Text = "Load Training File";
            button_load_train_file.UseVisualStyleBackColor = true;
            button_load_train_file.Click += button_load_file_Click;
            // 
            // button_normalize
            // 
            button_normalize.AutoSize = true;
            button_normalize.Location = new Point(359, 16);
            button_normalize.Name = "button_normalize";
            button_normalize.Size = new Size(94, 30);
            button_normalize.TabIndex = 2;
            button_normalize.Text = "Normalize";
            button_normalize.UseVisualStyleBackColor = true;
            button_normalize.Click += button_normalize_Click_1;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(radioButton_sigmoid);
            flowLayoutPanel1.Controls.Add(radioButton_tanh);
            flowLayoutPanel1.Location = new Point(472, 12);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(169, 37);
            flowLayoutPanel1.TabIndex = 3;
            // 
            // radioButton_sigmoid
            // 
            radioButton_sigmoid.AutoSize = true;
            radioButton_sigmoid.Location = new Point(3, 3);
            radioButton_sigmoid.Name = "radioButton_sigmoid";
            radioButton_sigmoid.Size = new Size(86, 24);
            radioButton_sigmoid.TabIndex = 0;
            radioButton_sigmoid.TabStop = true;
            radioButton_sigmoid.Text = "Sigmoid";
            radioButton_sigmoid.UseVisualStyleBackColor = true;
            // 
            // radioButton_tanh
            // 
            radioButton_tanh.AutoSize = true;
            radioButton_tanh.Location = new Point(95, 3);
            radioButton_tanh.Name = "radioButton_tanh";
            radioButton_tanh.Size = new Size(63, 24);
            radioButton_tanh.TabIndex = 1;
            radioButton_tanh.TabStop = true;
            radioButton_tanh.Text = "TanH";
            radioButton_tanh.UseVisualStyleBackColor = true;
            // 
            // button_train
            // 
            button_train.AutoSize = true;
            button_train.Location = new Point(686, 16);
            button_train.Name = "button_train";
            button_train.Size = new Size(94, 30);
            button_train.TabIndex = 4;
            button_train.Text = "Train";
            button_train.UseVisualStyleBackColor = true;
            button_train.Click += button_train_Click;
            // 
            // panel1
            // 
            panel1.BorderStyle = BorderStyle.FixedSingle;
            panel1.Controls.Add(textBox_max_error);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(textBox_learning_rate);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(textBox_max_epochs);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(textBox_hl_neurons);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(1193, 38);
            panel1.Name = "panel1";
            panel1.Size = new Size(250, 161);
            panel1.TabIndex = 5;
            // 
            // textBox_max_error
            // 
            textBox_max_error.Location = new Point(109, 112);
            textBox_max_error.Name = "textBox_max_error";
            textBox_max_error.Size = new Size(125, 27);
            textBox_max_error.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(30, 110);
            label4.Name = "label4";
            label4.Size = new Size(73, 20);
            label4.TabIndex = 6;
            label4.Text = "Max Error";
            // 
            // textBox_learning_rate
            // 
            textBox_learning_rate.Location = new Point(109, 79);
            textBox_learning_rate.Name = "textBox_learning_rate";
            textBox_learning_rate.Size = new Size(125, 27);
            textBox_learning_rate.TabIndex = 6;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(3, 78);
            label3.Name = "label3";
            label3.Size = new Size(100, 20);
            label3.TabIndex = 6;
            label3.Text = "Learning Rate";
            // 
            // textBox_max_epochs
            // 
            textBox_max_epochs.Location = new Point(109, 46);
            textBox_max_epochs.Name = "textBox_max_epochs";
            textBox_max_epochs.Size = new Size(125, 27);
            textBox_max_epochs.TabIndex = 6;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(17, 46);
            label2.Name = "label2";
            label2.Size = new Size(88, 20);
            label2.TabIndex = 6;
            label2.Text = "Max Epochs";
            // 
            // textBox_hl_neurons
            // 
            textBox_hl_neurons.Location = new Point(109, 12);
            textBox_hl_neurons.Name = "textBox_hl_neurons";
            textBox_hl_neurons.Size = new Size(125, 27);
            textBox_hl_neurons.TabIndex = 6;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(17, 15);
            label1.Name = "label1";
            label1.Size = new Size(86, 20);
            label1.TabIndex = 6;
            label1.Text = "HL Neurons";
            // 
            // button_test
            // 
            button_test.AutoSize = true;
            button_test.Location = new Point(815, 16);
            button_test.Name = "button_test";
            button_test.Size = new Size(94, 30);
            button_test.TabIndex = 10;
            button_test.Text = "Test";
            button_test.UseVisualStyleBackColor = true;
            button_test.Click += button_test_Click;
            // 
            // plotView_testing
            // 
            plotView_testing.Location = new Point(3, 504);
            plotView_testing.Name = "plotView_testing";
            plotView_testing.PanCursor = Cursors.Hand;
            plotView_testing.Size = new Size(906, 291);
            plotView_testing.TabIndex = 11;
            plotView_testing.Text = "Testing";
            plotView_testing.ZoomHorizontalCursor = Cursors.SizeWE;
            plotView_testing.ZoomRectangleCursor = Cursors.SizeNWSE;
            plotView_testing.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // button_load_model
            // 
            button_load_model.AutoSize = true;
            button_load_model.Location = new Point(1608, 85);
            button_load_model.Name = "button_load_model";
            button_load_model.Size = new Size(99, 30);
            button_load_model.TabIndex = 12;
            button_load_model.Text = "Load Model";
            button_load_model.UseVisualStyleBackColor = true;
            button_load_model.Click += button_load_model_Click;
            // 
            // button_save_model
            // 
            button_save_model.AutoSize = true;
            button_save_model.Location = new Point(1608, 38);
            button_save_model.Name = "button_save_model";
            button_save_model.Size = new Size(97, 30);
            button_save_model.TabIndex = 13;
            button_save_model.Text = "Save Model";
            button_save_model.UseVisualStyleBackColor = true;
            button_save_model.Click += button_save_model_Click;
            // 
            // button_real_test
            // 
            button_real_test.AutoSize = true;
            button_real_test.Location = new Point(1006, 20);
            button_real_test.Name = "button_real_test";
            button_real_test.Size = new Size(94, 30);
            button_real_test.TabIndex = 14;
            button_real_test.Text = "Real-Test";
            button_real_test.UseVisualStyleBackColor = true;
            button_real_test.Click += button_real_test_Click;
            // 
            // plotView_training
            // 
            plotView_training.AccessibleRole = AccessibleRole.None;
            plotView_training.Location = new Point(925, 218);
            plotView_training.Name = "plotView_training";
            plotView_training.PanCursor = Cursors.Hand;
            plotView_training.Size = new Size(906, 250);
            plotView_training.TabIndex = 9;
            plotView_training.Text = "Training";
            plotView_training.ZoomHorizontalCursor = Cursors.SizeWE;
            plotView_training.ZoomRectangleCursor = Cursors.SizeNWSE;
            plotView_training.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // plotView_matrix
            // 
            plotView_matrix.AccessibleRole = AccessibleRole.None;
            plotView_matrix.Location = new Point(925, 504);
            plotView_matrix.Name = "plotView_matrix";
            plotView_matrix.PanCursor = Cursors.Hand;
            plotView_matrix.Size = new Size(906, 291);
            plotView_matrix.TabIndex = 15;
            plotView_matrix.Text = "Training";
            plotView_matrix.ZoomHorizontalCursor = Cursors.SizeWE;
            plotView_matrix.ZoomRectangleCursor = Cursors.SizeNWSE;
            plotView_matrix.ZoomVerticalCursor = Cursors.SizeNS;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1843, 843);
            Controls.Add(plotView_matrix);
            Controls.Add(button_real_test);
            Controls.Add(button_save_model);
            Controls.Add(button_load_model);
            Controls.Add(plotView_testing);
            Controls.Add(button_test);
            Controls.Add(plotView_training);
            Controls.Add(panel1);
            Controls.Add(button_train);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(button_normalize);
            Controls.Add(button_load_train_file);
            Controls.Add(dataGridView_data);
            Name = "Form1";
            Text = "DDoS IDS";
            ((System.ComponentModel.ISupportInitialize)dataGridView_data).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView dataGridView_data;
        private Button button_load_train_file;
        private Button button_normalize;
        private FlowLayoutPanel flowLayoutPanel1;
        private RadioButton radioButton_sigmoid;
        private RadioButton radioButton_tanh;
        private Button button_train;
        private Panel panel1;
        private Label label1;
        private TextBox textBox_hl_neurons;
        private Label label2;
        private TextBox textBox_max_epochs;
        private Label label3;
        private TextBox textBox_learning_rate;
        private Label label4;
        private TextBox textBox_max_error;
        private Button button_test;
        private OxyPlot.WindowsForms.PlotView plotView_testing;
        private Button button_load_model;
        private Button button_save_model;
        private Button button_real_test;
        private OxyPlot.WindowsForms.PlotView plotView_training;
        private OxyPlot.WindowsForms.PlotView plotView_matrix;
    }
}
