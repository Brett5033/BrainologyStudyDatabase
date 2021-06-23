
namespace BrainologyStudyDatabase
{
    partial class ComboBoxControl
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
            this.CBC_CBX = new MetroFramework.Controls.MetroComboBox();
            this.CBC_LBL = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // CBC_CBX
            // 
            this.CBC_CBX.FormattingEnabled = true;
            this.CBC_CBX.ItemHeight = 24;
            this.CBC_CBX.Location = new System.Drawing.Point(3, 29);
            this.CBC_CBX.Name = "CBC_CBX";
            this.CBC_CBX.Size = new System.Drawing.Size(210, 30);
            this.CBC_CBX.TabIndex = 0;
            this.CBC_CBX.UseSelectable = true;
            // 
            // CBC_LBL
            // 
            this.CBC_LBL.AutoSize = true;
            this.CBC_LBL.Location = new System.Drawing.Point(3, 6);
            this.CBC_LBL.Name = "CBC_LBL";
            this.CBC_LBL.Size = new System.Drawing.Size(84, 20);
            this.CBC_LBL.TabIndex = 1;
            this.CBC_LBL.Text = "metroLabel1";
            // 
            // ComboBoxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.CBC_LBL);
            this.Controls.Add(this.CBC_CBX);
            this.Name = "ComboBoxControl";
            this.Size = new System.Drawing.Size(216, 62);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox CBC_CBX;
        private MetroFramework.Controls.MetroLabel CBC_LBL;
    }
}
