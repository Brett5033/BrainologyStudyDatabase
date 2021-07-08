
namespace BrainologyStudyDatabase
{
    partial class TextBoxControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TBC_LBL = new MetroFramework.Controls.MetroLabel();
            this.TBC_TXT = new MetroFramework.Controls.MetroTextBox();
            this.SuspendLayout();
            // 
            // TBC_LBL
            // 
            this.TBC_LBL.AutoSize = true;
            this.TBC_LBL.Location = new System.Drawing.Point(3, 6);
            this.TBC_LBL.Name = "TBC_LBL";
            this.TBC_LBL.Size = new System.Drawing.Size(84, 20);
            this.TBC_LBL.TabIndex = 1;
            this.TBC_LBL.Text = "metroLabel1";
            // 
            // TBC_TXT
            // 
            // 
            // 
            // 
            this.TBC_TXT.CustomButton.Image = null;
            this.TBC_TXT.CustomButton.Location = new System.Drawing.Point(188, 1);
            this.TBC_TXT.CustomButton.Name = "";
            this.TBC_TXT.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.TBC_TXT.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.TBC_TXT.CustomButton.TabIndex = 1;
            this.TBC_TXT.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.TBC_TXT.CustomButton.UseSelectable = true;
            this.TBC_TXT.CustomButton.Visible = false;
            this.TBC_TXT.Lines = new string[] {
        "metroTextBox1"};
            this.TBC_TXT.Location = new System.Drawing.Point(3, 36);
            this.TBC_TXT.MaxLength = 32767;
            this.TBC_TXT.Name = "TBC_TXT";
            this.TBC_TXT.PasswordChar = '\0';
            this.TBC_TXT.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.TBC_TXT.SelectedText = "";
            this.TBC_TXT.SelectionLength = 0;
            this.TBC_TXT.SelectionStart = 0;
            this.TBC_TXT.ShortcutsEnabled = true;
            this.TBC_TXT.Size = new System.Drawing.Size(210, 23);
            this.TBC_TXT.TabIndex = 2;
            this.TBC_TXT.Text = "metroTextBox1";
            this.TBC_TXT.UseSelectable = true;
            this.TBC_TXT.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.TBC_TXT.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // TextBoxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.TBC_TXT);
            this.Controls.Add(this.TBC_LBL);
            this.Name = "TextBoxControl";
            this.Size = new System.Drawing.Size(216, 62);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MetroFramework.Controls.MetroLabel TBC_LBL;
        private MetroFramework.Controls.MetroTextBox TBC_TXT;
    }
}
