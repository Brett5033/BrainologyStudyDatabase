using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrainologyStudyDatabase
{
    public partial class PasteForm : MetroFramework.Forms.MetroForm
    {

        public DatabaseEnums.TABLES currentTable;
        public HomeScreen parent;
        public DatabaseHandler db;
        public PasteForm()
        {
            InitializeComponent();
            currentTable = DatabaseEnums.TABLES.STUDY;
        }

        public void setParams(HomeScreen parent, DataTable tableSchema, DatabaseHandler db, DatabaseEnums.TABLES currentTable)
        {
            this.parent = parent;
            PF_DGData.DataSource = tableSchema.Clone();
            this.db = db;
            this.currentTable = currentTable;
        }


        private void PasteForm_Load(object sender, EventArgs e)
        {
            // Add comboboxes and format
            Console.WriteLine("\tPopulating Combo boxes and formatting columns");
            parent.populateDataGridView(PF_DGData);
            parent.formatDataGrid(PF_DGData);

            pasteData(PF_DGData);

            PF_BTNAcceptEntries.Enabled = validateCells(PF_DGData);
        }

        private bool pasteData(DataGridView dataGrid)
        {
            try
            {
                string s = Clipboard.GetText();

                Console.WriteLine("Pasting data into the format of " + currentTable + " -- Columns: " + dataGrid.Columns.Count);

                string[] lines = s.Replace("\n", "").Split('\r');

                int writeColCount = 0;
                List<int> foreignKeys = new List<int>();
                for (int i = 0; i < dataGrid.Columns.Count; i++)
                {
                    if (!dataGrid.Columns[i].ReadOnly)
                    {
                        writeColCount++;
                    }
                    if (DatabaseHandler.getValidValuesForForiegnKey(((DataTable)dataGrid.DataSource).Columns[i].ColumnName, currentTable.ToString()) != "")
                    {
                        // Foriegn Key found
                        foreignKeys.Add(i);
                        Console.WriteLine("FK at " + i);
                    }

                }

                string[] fields = lines[0].Split('\t');
                if (fields.Length != writeColCount)
                {
                    // Data Mismatch, dont proceed
                    MessageBox.Show("Data Mismatch found on clipboard data.\n" + currentTable + " requires " + writeColCount + " data entries per row, " + fields.Length + " provided", "Data Mismatch");
                    return false;
                }

                Console.WriteLine("\t" + currentTable + " - Columns: " + dataGrid.Columns.Count + " with Read Only: " + (dataGrid.Columns.Count - writeColCount));

                int col = 0;

                DataTable table = (DataTable)dataGrid.DataSource;

                foreach (string item in lines)
                {
                    if (item == "")
                        continue;

                    DataRow newRow = table.NewRow();

                    fields = item.Split('\t');
                    foreach (string f in fields)
                    {
                        Console.WriteLine("\t" + f);
                        while (dataGrid.Columns[col].ReadOnly)
                        {
                            // Skips input on read only lines
                            col++;
                        }
                        Console.WriteLine("\t\tWriting to col: " + dataGrid.Columns[col]);
                        //DI_DGInput[col, row].Value = f;
                        if (foreignKeys.Contains(col))
                        {
                            // TEMP SKIP THIS ------
                            int fkIndex = 1;
                            foreach(DataRowView comboRow in ((DataGridViewComboBoxColumn)dataGrid.Columns[col]).Items)
                            {
                                DataRow dataR = comboRow.Row;
                                foreach(object obj in dataR.ItemArray)
                                {
                                    Console.WriteLine("\t\t\tO:" + obj.ToString());
                                }
                                if (dataR.ItemArray.Contains(f)) // Might crash from type mismatch
                                {
                                    // Sets the ID to the 0th position of the combobox (ie the ID value)
                                    fkIndex = (int)dataR.ItemArray[0]; 
                                    Console.WriteLine("Found matching row entry: " + f + " = (" + dataR.ToString() + ")");
                                    break;
                                }
                                else
                                {
                                    // THIS COULD CRASH IF THERE IS NO INPUT, AW WELL
                                    fkIndex = -1;

                                }
                            }

                            // ---------------------
                            //int fkIndex = db.getValueFromForeignKey(f, currentTable, dataGrid.Columns[col].Name);
                            //Console.WriteLine("\t\t" + f + " -> " + fkIndex);
                            newRow[col] = fkIndex;
                        }
                        else 
                        {
                            newRow[col] = f;
                        }

                        col++;
                    }
                    col = 0;
                    table.Rows.Add(newRow);

                    //table.AcceptChanges();
                }

                dataGrid.DataSource = table;

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine("Crash in PasteForm, Paste Data: Table = " + currentTable);
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        private bool validateCells(DataGridView dataGrid)
        {
            bool allCellsValid = true;
            try
            {
                for (int row = 0; row < dataGrid.Rows.Count-1; row++)
                {
                    int colCounter = 0;
                    foreach (DataGridViewCell cell in dataGrid.Rows[row].Cells)
                    {
                        if (cell.ErrorText.Length > 0 || (DatabaseHandler.getValidValuesForForiegnKey(((DataTable)dataGrid.DataSource).Columns[colCounter].ColumnName, currentTable.ToString()) != "" && cell.Value == null))
                        {
                            allCellsValid = false;
                            Console.WriteLine(cell.ToString() + " failed the validate");
                            cell.Style.ForeColor = Color.Red;
                        }
                        colCounter++;
                    }
                }
                return allCellsValid;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

        }

        private void PF_BTNAcceptEntries_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Paste Form Data Grid: ");
            parent.printDataTable((DataTable)PF_DGData.DataSource);
            parent.MergePasteData(PF_DGData, currentTable);
            this.Close();
        }

        private void PF_BTNValidate_Click(object sender, EventArgs e)
        {
            PF_BTNAcceptEntries.Enabled = validateCells(PF_DGData);
        }

        private void PF_DGData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Console.WriteLine("Data Error on col: " + e.ColumnIndex + " :: row: " + e.RowIndex);
        }
    }
}
