
namespace BrainologyStudyDatabase
{
    partial class PasteForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.PF_DGData = new System.Windows.Forms.DataGridView();
            this.PF_BTNAcceptEntries = new MetroFramework.Controls.MetroButton();
            this.PF_BTNValidate = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.PF_DGData)).BeginInit();
            this.SuspendLayout();
            // 
            // PF_DGData
            // 
            this.PF_DGData.BackgroundColor = System.Drawing.Color.White;
            this.PF_DGData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PF_DGData.Location = new System.Drawing.Point(23, 63);
            this.PF_DGData.Name = "PF_DGData";
            this.PF_DGData.RowHeadersWidth = 51;
            this.PF_DGData.RowTemplate.Height = 24;
            this.PF_DGData.Size = new System.Drawing.Size(754, 364);
            this.PF_DGData.TabIndex = 0;
            this.PF_DGData.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.PF_DGData_DataError);
            // 
            // PF_BTNAcceptEntries
            // 
            this.PF_BTNAcceptEntries.Location = new System.Drawing.Point(668, 433);
            this.PF_BTNAcceptEntries.Name = "PF_BTNAcceptEntries";
            this.PF_BTNAcceptEntries.Size = new System.Drawing.Size(109, 30);
            this.PF_BTNAcceptEntries.TabIndex = 1;
            this.PF_BTNAcceptEntries.Text = "Accept Entries";
            this.PF_BTNAcceptEntries.UseSelectable = true;
            this.PF_BTNAcceptEntries.Click += new System.EventHandler(this.PF_BTNAcceptEntries_Click);
            // 
            // PF_BTNValidate
            // 
            this.PF_BTNValidate.Location = new System.Drawing.Point(535, 433);
            this.PF_BTNValidate.Name = "PF_BTNValidate";
            this.PF_BTNValidate.Size = new System.Drawing.Size(109, 30);
            this.PF_BTNValidate.TabIndex = 2;
            this.PF_BTNValidate.Text = "Validate";
            this.PF_BTNValidate.UseSelectable = true;
            this.PF_BTNValidate.Click += new System.EventHandler(this.PF_BTNValidate_Click);
            // 
            // PasteForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 472);
            this.Controls.Add(this.PF_BTNValidate);
            this.Controls.Add(this.PF_BTNAcceptEntries);
            this.Controls.Add(this.PF_DGData);
            this.Name = "PasteForm";
            this.Style = MetroFramework.MetroColorStyle.Magenta;
            this.Text = "Paste From Clipboard";
            this.Load += new System.EventHandler(this.PasteForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.PF_DGData)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView PF_DGData;
        private MetroFramework.Controls.MetroButton PF_BTNAcceptEntries;
        private MetroFramework.Controls.MetroButton PF_BTNValidate;
    }
}