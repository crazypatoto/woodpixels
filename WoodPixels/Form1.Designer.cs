namespace WoodPixels
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.imageBox_Target = new Emgu.CV.UI.ImageBox();
            this.imageBox_Texture = new Emgu.CV.UI.ImageBox();
            this.groupBox_Target = new System.Windows.Forms.GroupBox();
            this.button_LoadTarget = new System.Windows.Forms.Button();
            this.groupBox_Texture = new System.Windows.Forms.GroupBox();
            this.button_LoadTexture = new System.Windows.Forms.Button();
            this.imageBox_template = new Emgu.CV.UI.ImageBox();
            this.imageBox_match = new Emgu.CV.UI.ImageBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.button_StartMatching = new System.Windows.Forms.Button();
            this.groupBox_match = new System.Windows.Forms.GroupBox();
            this.progressBar_match = new System.Windows.Forms.ProgressBar();
            this.imageBox_Result = new Emgu.CV.UI.ImageBox();
            this.groupBox_Parameters = new System.Windows.Forms.GroupBox();
            this.button_hist = new System.Windows.Forms.Button();
            this.trackBar_hist = new System.Windows.Forms.TrackBar();
            this.label_hist = new System.Windows.Forms.Label();
            this.trackBar_scale = new System.Windows.Forms.TrackBar();
            this.label_scale = new System.Windows.Forms.Label();
            this.trackBar_Rotations = new System.Windows.Forms.TrackBar();
            this.label_Rotations = new System.Windows.Forms.Label();
            this.trackBar_size = new System.Windows.Forms.TrackBar();
            this.label_size = new System.Windows.Forms.Label();
            this.groupBox_hist = new System.Windows.Forms.GroupBox();
            this.imageBox_hist = new Emgu.CV.UI.ImageBox();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_Target)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_Texture)).BeginInit();
            this.groupBox_Target.SuspendLayout();
            this.groupBox_Texture.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_template)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_match)).BeginInit();
            this.groupBox_match.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_Result)).BeginInit();
            this.groupBox_Parameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_hist)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_scale)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Rotations)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_size)).BeginInit();
            this.groupBox_hist.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_hist)).BeginInit();
            this.SuspendLayout();
            // 
            // imageBox_Target
            // 
            this.imageBox_Target.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox_Target.Location = new System.Drawing.Point(10, 17);
            this.imageBox_Target.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageBox_Target.Name = "imageBox_Target";
            this.imageBox_Target.Size = new System.Drawing.Size(320, 266);
            this.imageBox_Target.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox_Target.TabIndex = 2;
            this.imageBox_Target.TabStop = false;
            // 
            // imageBox_Texture
            // 
            this.imageBox_Texture.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox_Texture.Cursor = System.Windows.Forms.Cursors.Cross;
            this.imageBox_Texture.Location = new System.Drawing.Point(11, 19);
            this.imageBox_Texture.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageBox_Texture.Name = "imageBox_Texture";
            this.imageBox_Texture.Size = new System.Drawing.Size(320, 266);
            this.imageBox_Texture.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox_Texture.TabIndex = 3;
            this.imageBox_Texture.TabStop = false;
            // 
            // groupBox_Target
            // 
            this.groupBox_Target.Controls.Add(this.button_LoadTarget);
            this.groupBox_Target.Controls.Add(this.imageBox_Target);
            this.groupBox_Target.Location = new System.Drawing.Point(12, 13);
            this.groupBox_Target.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox_Target.Name = "groupBox_Target";
            this.groupBox_Target.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox_Target.Size = new System.Drawing.Size(343, 329);
            this.groupBox_Target.TabIndex = 4;
            this.groupBox_Target.TabStop = false;
            this.groupBox_Target.Text = "Target";
            // 
            // button_LoadTarget
            // 
            this.button_LoadTarget.Location = new System.Drawing.Point(10, 291);
            this.button_LoadTarget.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_LoadTarget.Name = "button_LoadTarget";
            this.button_LoadTarget.Size = new System.Drawing.Size(320, 31);
            this.button_LoadTarget.TabIndex = 3;
            this.button_LoadTarget.Text = "Load Target";
            this.button_LoadTarget.UseVisualStyleBackColor = true;
            this.button_LoadTarget.Click += new System.EventHandler(this.button_LoadTarget_Click);
            // 
            // groupBox_Texture
            // 
            this.groupBox_Texture.Controls.Add(this.button_LoadTexture);
            this.groupBox_Texture.Controls.Add(this.imageBox_Texture);
            this.groupBox_Texture.Location = new System.Drawing.Point(361, 13);
            this.groupBox_Texture.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox_Texture.Name = "groupBox_Texture";
            this.groupBox_Texture.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox_Texture.Size = new System.Drawing.Size(343, 329);
            this.groupBox_Texture.TabIndex = 5;
            this.groupBox_Texture.TabStop = false;
            this.groupBox_Texture.Text = "Texture";
            // 
            // button_LoadTexture
            // 
            this.button_LoadTexture.Location = new System.Drawing.Point(11, 293);
            this.button_LoadTexture.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_LoadTexture.Name = "button_LoadTexture";
            this.button_LoadTexture.Size = new System.Drawing.Size(320, 31);
            this.button_LoadTexture.TabIndex = 4;
            this.button_LoadTexture.Text = "Load Texture";
            this.button_LoadTexture.UseVisualStyleBackColor = true;
            this.button_LoadTexture.Click += new System.EventHandler(this.button_LoadTexture_Click);
            // 
            // imageBox_template
            // 
            this.imageBox_template.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox_template.Location = new System.Drawing.Point(21, 23);
            this.imageBox_template.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageBox_template.Name = "imageBox_template";
            this.imageBox_template.Size = new System.Drawing.Size(150, 150);
            this.imageBox_template.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox_template.TabIndex = 2;
            this.imageBox_template.TabStop = false;
            // 
            // imageBox_match
            // 
            this.imageBox_match.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox_match.Location = new System.Drawing.Point(177, 23);
            this.imageBox_match.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageBox_match.Name = "imageBox_match";
            this.imageBox_match.Size = new System.Drawing.Size(150, 150);
            this.imageBox_match.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox_match.TabIndex = 6;
            this.imageBox_match.TabStop = false;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog1";
            // 
            // button_StartMatching
            // 
            this.button_StartMatching.Location = new System.Drawing.Point(20, 218);
            this.button_StartMatching.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_StartMatching.Name = "button_StartMatching";
            this.button_StartMatching.Size = new System.Drawing.Size(307, 70);
            this.button_StartMatching.TabIndex = 7;
            this.button_StartMatching.Text = "Start Matching";
            this.button_StartMatching.UseVisualStyleBackColor = true;
            this.button_StartMatching.Click += new System.EventHandler(this.button_StartMatching_Click);
            // 
            // groupBox_match
            // 
            this.groupBox_match.Controls.Add(this.progressBar_match);
            this.groupBox_match.Controls.Add(this.imageBox_template);
            this.groupBox_match.Controls.Add(this.imageBox_Result);
            this.groupBox_match.Controls.Add(this.button_StartMatching);
            this.groupBox_match.Controls.Add(this.imageBox_match);
            this.groupBox_match.Location = new System.Drawing.Point(12, 349);
            this.groupBox_match.Name = "groupBox_match";
            this.groupBox_match.Size = new System.Drawing.Size(692, 297);
            this.groupBox_match.TabIndex = 8;
            this.groupBox_match.TabStop = false;
            this.groupBox_match.Text = "Match";
            // 
            // progressBar_match
            // 
            this.progressBar_match.Location = new System.Drawing.Point(21, 180);
            this.progressBar_match.Name = "progressBar_match";
            this.progressBar_match.Size = new System.Drawing.Size(306, 31);
            this.progressBar_match.TabIndex = 10;
            // 
            // imageBox_Result
            // 
            this.imageBox_Result.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox_Result.Location = new System.Drawing.Point(349, 22);
            this.imageBox_Result.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageBox_Result.Name = "imageBox_Result";
            this.imageBox_Result.Size = new System.Drawing.Size(320, 266);
            this.imageBox_Result.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox_Result.TabIndex = 2;
            this.imageBox_Result.TabStop = false;
            // 
            // groupBox_Parameters
            // 
            this.groupBox_Parameters.Controls.Add(this.button_hist);
            this.groupBox_Parameters.Controls.Add(this.trackBar_hist);
            this.groupBox_Parameters.Controls.Add(this.label_hist);
            this.groupBox_Parameters.Controls.Add(this.trackBar_scale);
            this.groupBox_Parameters.Controls.Add(this.label_scale);
            this.groupBox_Parameters.Controls.Add(this.trackBar_Rotations);
            this.groupBox_Parameters.Controls.Add(this.label_Rotations);
            this.groupBox_Parameters.Controls.Add(this.trackBar_size);
            this.groupBox_Parameters.Controls.Add(this.label_size);
            this.groupBox_Parameters.Location = new System.Drawing.Point(710, 349);
            this.groupBox_Parameters.Name = "groupBox_Parameters";
            this.groupBox_Parameters.Size = new System.Drawing.Size(343, 297);
            this.groupBox_Parameters.TabIndex = 10;
            this.groupBox_Parameters.TabStop = false;
            this.groupBox_Parameters.Text = "Paremeters Setting";
            // 
            // button_hist
            // 
            this.button_hist.Location = new System.Drawing.Point(11, 218);
            this.button_hist.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_hist.Name = "button_hist";
            this.button_hist.Size = new System.Drawing.Size(320, 70);
            this.button_hist.TabIndex = 8;
            this.button_hist.Text = "Preview Histogram Matching Result";
            this.button_hist.UseVisualStyleBackColor = true;
            this.button_hist.Click += new System.EventHandler(this.button_hist_Click);
            // 
            // trackBar_hist
            // 
            this.trackBar_hist.LargeChange = 2;
            this.trackBar_hist.Location = new System.Drawing.Point(168, 175);
            this.trackBar_hist.Name = "trackBar_hist";
            this.trackBar_hist.Size = new System.Drawing.Size(163, 45);
            this.trackBar_hist.TabIndex = 7;
            this.trackBar_hist.Value = 5;
            this.trackBar_hist.Scroll += new System.EventHandler(this.trackBar_hist_Scroll);
            // 
            // label_hist
            // 
            this.label_hist.AutoSize = true;
            this.label_hist.Location = new System.Drawing.Point(12, 185);
            this.label_hist.Name = "label_hist";
            this.label_hist.Size = new System.Drawing.Size(148, 16);
            this.label_hist.TabIndex = 6;
            this.label_hist.Text = "Hist. match weight (1.0x):";
            // 
            // trackBar_scale
            // 
            this.trackBar_scale.LargeChange = 2;
            this.trackBar_scale.Location = new System.Drawing.Point(168, 124);
            this.trackBar_scale.Maximum = 30;
            this.trackBar_scale.Minimum = 5;
            this.trackBar_scale.Name = "trackBar_scale";
            this.trackBar_scale.Size = new System.Drawing.Size(163, 45);
            this.trackBar_scale.TabIndex = 5;
            this.trackBar_scale.Value = 10;
            this.trackBar_scale.Scroll += new System.EventHandler(this.trackBar_scale_Scroll);
            // 
            // label_scale
            // 
            this.label_scale.AutoSize = true;
            this.label_scale.Location = new System.Drawing.Point(12, 134);
            this.label_scale.Name = "label_scale";
            this.label_scale.Size = new System.Drawing.Size(150, 16);
            this.label_scale.TabIndex = 4;
            this.label_scale.Text = "Taget Image Scale (1.0x):";
            // 
            // trackBar_Rotations
            // 
            this.trackBar_Rotations.LargeChange = 2;
            this.trackBar_Rotations.Location = new System.Drawing.Point(168, 73);
            this.trackBar_Rotations.Maximum = 36;
            this.trackBar_Rotations.Minimum = 1;
            this.trackBar_Rotations.Name = "trackBar_Rotations";
            this.trackBar_Rotations.Size = new System.Drawing.Size(163, 45);
            this.trackBar_Rotations.TabIndex = 3;
            this.trackBar_Rotations.Value = 18;
            this.trackBar_Rotations.Scroll += new System.EventHandler(this.trackBar_Rotations_Scroll);
            // 
            // label_Rotations
            // 
            this.label_Rotations.AutoSize = true;
            this.label_Rotations.Location = new System.Drawing.Point(12, 83);
            this.label_Rotations.Name = "label_Rotations";
            this.label_Rotations.Size = new System.Drawing.Size(155, 16);
            this.label_Rotations.TabIndex = 2;
            this.label_Rotations.Text = "Number of Rotations (16):";
            // 
            // trackBar_size
            // 
            this.trackBar_size.LargeChange = 4;
            this.trackBar_size.Location = new System.Drawing.Point(168, 22);
            this.trackBar_size.Maximum = 128;
            this.trackBar_size.Minimum = 16;
            this.trackBar_size.Name = "trackBar_size";
            this.trackBar_size.Size = new System.Drawing.Size(163, 45);
            this.trackBar_size.SmallChange = 2;
            this.trackBar_size.TabIndex = 1;
            this.trackBar_size.TickFrequency = 2;
            this.trackBar_size.Value = 64;
            this.trackBar_size.Scroll += new System.EventHandler(this.trackBar_size_Scroll);
            // 
            // label_size
            // 
            this.label_size.AutoSize = true;
            this.label_size.Location = new System.Drawing.Point(41, 32);
            this.label_size.Name = "label_size";
            this.label_size.Size = new System.Drawing.Size(126, 16);
            this.label_size.TabIndex = 0;
            this.label_size.Text = "Patch Size (128*128):";
            // 
            // groupBox_hist
            // 
            this.groupBox_hist.Controls.Add(this.imageBox_hist);
            this.groupBox_hist.Location = new System.Drawing.Point(710, 13);
            this.groupBox_hist.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox_hist.Name = "groupBox_hist";
            this.groupBox_hist.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox_hist.Size = new System.Drawing.Size(343, 329);
            this.groupBox_hist.TabIndex = 11;
            this.groupBox_hist.TabStop = false;
            this.groupBox_hist.Text = "Histogram matching";
            // 
            // imageBox_hist
            // 
            this.imageBox_hist.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.imageBox_hist.Location = new System.Drawing.Point(11, 19);
            this.imageBox_hist.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.imageBox_hist.Name = "imageBox_hist";
            this.imageBox_hist.Size = new System.Drawing.Size(320, 302);
            this.imageBox_hist.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imageBox_hist.TabIndex = 3;
            this.imageBox_hist.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 658);
            this.Controls.Add(this.groupBox_hist);
            this.Controls.Add(this.groupBox_Parameters);
            this.Controls.Add(this.groupBox_match);
            this.Controls.Add(this.groupBox_Texture);
            this.Controls.Add(this.groupBox_Target);
            this.Font = new System.Drawing.Font("微軟正黑體", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "WoodPixel M10907K06";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_Target)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_Texture)).EndInit();
            this.groupBox_Target.ResumeLayout(false);
            this.groupBox_Texture.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_template)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_match)).EndInit();
            this.groupBox_match.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_Result)).EndInit();
            this.groupBox_Parameters.ResumeLayout(false);
            this.groupBox_Parameters.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_hist)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_scale)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Rotations)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_size)).EndInit();
            this.groupBox_hist.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.imageBox_hist)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Emgu.CV.UI.ImageBox imageBox_Target;
        private Emgu.CV.UI.ImageBox imageBox_Texture;
        private System.Windows.Forms.GroupBox groupBox_Target;
        private System.Windows.Forms.GroupBox groupBox_Texture;
        private Emgu.CV.UI.ImageBox imageBox_template;
        private Emgu.CV.UI.ImageBox imageBox_match;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button button_StartMatching;
        private System.Windows.Forms.Button button_LoadTarget;
        private System.Windows.Forms.Button button_LoadTexture;
        private System.Windows.Forms.GroupBox groupBox_match;
        private Emgu.CV.UI.ImageBox imageBox_Result;
        private System.Windows.Forms.ProgressBar progressBar_match;
        private System.Windows.Forms.GroupBox groupBox_Parameters;
        private System.Windows.Forms.TrackBar trackBar_size;
        private System.Windows.Forms.Label label_size;
        private System.Windows.Forms.TrackBar trackBar_Rotations;
        private System.Windows.Forms.Label label_Rotations;
        private System.Windows.Forms.TrackBar trackBar_scale;
        private System.Windows.Forms.Label label_scale;
        private System.Windows.Forms.GroupBox groupBox_hist;
        private Emgu.CV.UI.ImageBox imageBox_hist;
        private System.Windows.Forms.TrackBar trackBar_hist;
        private System.Windows.Forms.Label label_hist;
        private System.Windows.Forms.Button button_hist;
    }
}

