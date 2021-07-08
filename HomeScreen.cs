using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrainologyStudyDatabase
{
    public enum MainTabSelection
    {
        Hub,
        QueryData,
        DataInput,
        SQLScript
        
    }

    public enum HubTabSelection
    {
        Landing,
        Study,
        Participant,
        Session,
        Location
    }

    public enum SegmentTabSelection
    {
        Segments,
        AddSegment
    }

    public partial class HomeScreen : MetroFramework.Forms.MetroForm
    {
        public DatabaseHandler db;

        public PromptHandler ph;

        #region ScreenLoad
        public HomeScreen()
        {
            InitializeComponent();
        }

        private void HomeScreen_Load(object sender, EventArgs e)
        {
            try
            {
                db = new DatabaseHandler();
                ph = new PromptHandler();

                // Load Tab Pages
                //LoadHub();
                LoadDataInput();
                LoadQueryData();

                TabControlMain.SelectedIndex = 0;
                H_TABControl.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void HomeScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //MessageBox.Show("Connection Closed!");

                // Check Changes
                HandleDIChangesMade();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #endregion


        /// <summary>
        /// Occurs when the tab is selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TabControlMain_Selected(object sender, TabControlEventArgs e)
        {
            // Switch over tab pages to determine which need load/refresh action
            switch (e.TabPageIndex)
            {
                case (int)MainTabSelection.Hub:
                    {
                        LoadHub();
                        H_TABControl.SelectedIndex = 0;
                    }
                    break;

                case (int)MainTabSelection.QueryData:
                    {
                        LoadQueryData();

                    }
                    break;

                case (int)MainTabSelection.DataInput:
                    {
                        LoadDataInput();
                    }
                    break;
                case (int)MainTabSelection.SQLScript:
                    {


                    }
                    break;
            }
        }

        #region Hub


        private void LoadHub()
        {
            switch (H_TABControl.SelectedIndex)
            {
                case (int)HubTabSelection.Landing:
                    {
                        LoadLanding();
                    }
                    break;
                case (int)HubTabSelection.Study:
                    {
                        H_S_PopulateStudy();
                    }
                    break;
                case (int)HubTabSelection.Participant:
                    {
                        LoadParticipants();
                    }
                    break;
                case (int)HubTabSelection.Session:
                    {
                        LoadSessions();
                    }
                    break;
                case (int)HubTabSelection.Location:
                    {
                        LoadLocations();
                    }
                    break;
            }
        }

        private void H_TABControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadHub();
        }

        #region Landing

        private void LoadLanding()
        {

        }

        #endregion

        #region Study

        /// <summary>
        /// Populate (fill CBX) the Study select with all studies
        /// </summary>
        private void H_S_PopulateStudy(int selectedIndexOverride = 0)
        {
            Console.WriteLine("Loading Study Tab");
            // Populate Study column
            string studyQuery = "SELECT STUDY_ID, NAME, TYPE, START_DATE, END_DATE FROM STUDY";
            DataTable result = db.compileQuery(studyQuery);

            H_S_CBXStudySelect.DataSource = result.DefaultView;
            H_S_CBXStudySelect.DisplayMember = "NAME";
            H_S_CBXStudySelect.ValueMember = "STUDY_ID";
            

            // Set Study Index 0
            if (result.Rows.Count > 0)
            {
                // Trigger changed Event
                H_S_CBXStudySelect.SelectedIndex = selectedIndexOverride;

            }
        }

        /// <summary>
        /// A new study has been selected in the combobox, needs to be displayed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void H_S_CBXStudySelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRow selectedStudy = ((DataView)H_S_CBXStudySelect.DataSource).Table.Rows[H_S_CBXStudySelect.SelectedIndex];
            SelectedStudyID = (int)selectedStudy.ItemArray[0];
            selectedStudyIndex = H_S_CBXStudySelect.SelectedIndex;

            CreatingNewStudy = false;

            // Display the study
            H_S_displayStudy(selectedStudy);

            // Populate Versions (gets the study id from the selected data row)
            H_S_populateStudyVersions((int)(selectedStudy.ItemArray[0]));
        }

        /// <summary>
        /// Holds the STUDY_ID of the currently displayed study
        /// </summary>
        private int SelectedStudyID;

        /// <summary>
        /// The Index within the cbx study select, used to restore the proper display
        /// </summary>
        private int selectedStudyIndex;

        /// <summary>
        /// Determines if a new study should be created when the save study button is pressed
        /// Will be turned off when a different study is selected
        /// </summary>
        private bool CreatingNewStudy = false;

        /// <summary>
        /// Takes a datarow of the study table and displays its data
        /// </summary>
        /// <param name="study"></param>
        private void H_S_displayStudy(DataRow study)
        {
            // TODO: Might need to access based on column name
            foreach(object o in study.ItemArray)
            {
                Console.WriteLine(o.ToString());
            }

            H_S_TXTStudyName.Text = study.ItemArray[1].ToString();
            H_S_CBXStudyType.SelectedIndex = (int)study.ItemArray[2] - 1;

            if(study.ItemArray[3].ToString() != "")
                H_S_DTPStart_Date.Value = Convert.ToDateTime(study.ItemArray[3].ToString());

            if(study.ItemArray[4].ToString() != "")
                H_S_DTPEnd_Date.Value = Convert.ToDateTime(study.ItemArray[4].ToString());
        }

        /// <summary>
        /// Saves or updates study data of the currently displayed study
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void S_H_BTNSaveStudy_Click(object sender, EventArgs e)
        {
            try
            {

                string sqlCommand;
                if (!CreatingNewStudy)
                    sqlCommand = "UPDATE STUDY SET NAME = @ColNAME, TYPE = @ColTYPE, START_DATE = @ColSTART_DATE, END_DATE = @ColEND_DATE WHERE STUDY_ID = " + SelectedStudyID;
                else
                    sqlCommand = "INSERT INTO STUDY (NAME, TYPE, START_DATE, END_DATE) VALUES (@ColNAME, @ColTYPE, @ColSTART_DATE, @ColEND_DATE)";

                // TODO: 
                // Needs parameterizied query with data values pulled from the text boxes

                if (H_S_TXTStudyName.Text == "" || H_S_CBXStudyType.SelectedIndex < 0)
                {
                    MessageBox.Show("Please fill all required fields before saving");
                    return;
                }
                // Build Parameter list to send to query
                SqlParameter[] parameters = new SqlParameter[4];
                
                parameters[0] = new SqlParameter("@ColNAME", H_S_TXTStudyName.Text);

                int typeSelection = H_S_CBXStudyType.SelectedIndex + 1; // Shift up to match format
                parameters[1] = new SqlParameter("@ColTYPE", typeSelection);
                parameters[2] = new SqlParameter("@ColSTART_DATE", H_S_DTPStart_Date.Value);
                parameters[3] = new SqlParameter("@ColEND_DATE", H_S_DTPEnd_Date.Value);

                Console.WriteLine("Save Study Parameters Initializied, execute command");
                db.executeCommand(sqlCommand, parameters);
                Console.WriteLine("Study Saved");
                MessageBox.Show("Study Saved");
                H_S_PopulateStudy(selectedStudyIndex);

            }
            catch(Exception ex)
            {
                MessageBox.Show("Error occured when saving study data:\n" + ex.Message);
            }
        }

        /// <summary>
        /// Create a new study and clear the study display/input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void S_H_BTNNewStudy_Click(object sender, EventArgs e)
        {
            CreatingNewStudy = true;
            H_S_TXTStudyName.Text = "";
            H_S_CBXStudyType.SelectedIndex = 0;
            H_S_DTPStart_Date.Value = DateTime.Now;
            H_S_DTPEnd_Date.Value = DateTime.Now;
            H_S_CBXVersionSelect.Enabled = false;
        }

        /// <summary>
        /// Populate (Fill CBX) the study versions for the given study
        /// </summary>
        /// <param name="studyID"></param>
        private void H_S_populateStudyVersions(int studyID, int selectedIndexOverride = 0)
        {
            string query = string.Format("SELECT VERSION_ID, STUDY_ID, VERSION_NUM, DESCRIPTION FROM STUDY_VERSION WHERE STUDY_ID = {0}", studyID);
            DataTable result = db.compileQuery(query);

            if(result.Rows.Count > 0)
            {
                H_S_CBXVersionSelect.Enabled = true;
                H_S_TCSegments.Enabled = true;
                H_S_CBXVersionSelect.DataSource = result;
                H_S_CBXVersionSelect.ValueMember = "VERSION_ID";
                H_S_CBXVersionSelect.DisplayMember = "DESCRIPTION";

                // Select Index 0
                H_S_CBXVersionSelect.SelectedIndex = selectedIndexOverride;
                H_S_CBXVersionSelect_SelectedIndexChanged(null, null);

            }
            else
            {   // TODO: This is being called on a study that has 1 study version
                // No versions for the current study, disable cbx and clear segments
                H_S_CBXVersionSelect.Enabled = false;
                H_S_TXTVersionName.Text = "None";
                H_S_TCSegments.Enabled = false;
                //H_S_CBXVersionSelect.Items.Clear();
                H_S_displaySegments(-1);
            }
        }

        /// <summary>
        /// A new version is selected in the Version Select Combobox, displays new version and associated segments 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void H_S_CBXVersionSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Selected row
            DataRow selectedRow = ((DataTable)H_S_CBXVersionSelect.DataSource).Rows[H_S_CBXVersionSelect.SelectedIndex];
            selectedVersionID = (int)selectedRow.ItemArray[0];
            selectedVersionIndex = H_S_CBXVersionSelect.SelectedIndex;

            creatingNewVersion = false;

            // Display new version
            H_S_displayVersion(selectedRow);

            // Display segments of the version

            int version_ID = (int)selectedRow.ItemArray[0];
            Console.WriteLine("Displaying segments for version: " + version_ID);
            H_S_displaySegments(version_ID);
        }


        /// <summary>
        /// Displays a new version to the version panel
        /// </summary>
        /// <param name="version"></param>
        private void H_S_displayVersion(DataRow version)
        {
            // TODO: Might need to access based on column name

            H_S_TXTVersionName.Text = version.ItemArray[3].ToString();
            H_S_TXTVersionNumber.Text = version.ItemArray[2].ToString();
        }

        /// <summary>
        /// The VERSION_ID of the currently selected study version
        /// </summary>
        private int selectedVersionID;

        /// <summary>
        /// The Index within the cbx version select, used to restore the proper display
        /// </summary>
        private int selectedVersionIndex;

        private bool creatingNewVersion = false;

        private void H_SBTNSaveVersion_Click(object sender, EventArgs e)
        {
            try
            {
                string sqlCommand;
                if (!creatingNewVersion)
                    sqlCommand = "UPDATE STUDY_VERSION SET STUDY_ID = @ColSTUDY_ID, VERSION_NUM = @ColVERSION_NUM, DESCRIPTION = @ColDESCRIPTION WHERE VERSION_ID = " + selectedVersionID;
                else
                    sqlCommand = "INSERT INTO STUDY_VERSION (STUDY_ID, VERSION_NUM, DESCRIPTION) VALUES (@ColSTUDY_ID, @ColVERSION_NUM, @ColDESCRIPTION)";

                // TODO: 
                // Needs parameterizied query with data values pulled from the text boxes

                if (H_S_TXTVersionName.Text == "" || H_S_TXTVersionNumber.Text == "")
                {
                    MessageBox.Show("Please fill all required fields before saving");
                    return;
                }
                // Build Parameter list to send to query
                SqlParameter[] parameters = new SqlParameter[3];

                parameters[0] = new SqlParameter("@ColSTUDY_ID", SelectedStudyID);
                parameters[1] = new SqlParameter("@ColVERSION_NUM", H_S_TXTVersionNumber.Text);
                parameters[2] = new SqlParameter("@ColDESCRIPTION", H_S_TXTVersionName.Text);

                Console.WriteLine("Save Version Parameters Initializied, execute command");
                db.executeCommand(sqlCommand, parameters);
                Console.WriteLine("Version Saved");
                MessageBox.Show("Version Saved");
                H_S_populateStudyVersions(SelectedStudyID, selectedVersionIndex);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occured when saving version data:\n" + ex.Message);
            }
        }

        private void H_SBTNNewVersion_Click(object sender, EventArgs e)
        {
            creatingNewVersion = true;
            H_S_TXTVersionName.Text = "";
            H_S_TXTVersionNumber.Text = "";
        }


        /// <summary>
        /// Populate the segments datagrid with all segments that are in the selected version
        /// </summary>
        /// <param name="versionID"></param>
        private void H_S_displaySegments(int versionID)
        {
            string query = string.Format("SELECT SEGMENT_ID, VERSION_ID, NAME, DURATION, SOURCE FROM SEGMENT WHERE VERSION_ID = {0}", versionID);
            DataTable result = db.compileQuery(query);

            //if (result.Rows.Count > 0)
            //{
                H_S_S_SegmentView.DataSource = result;

                // Remove ID columns hopfully
                // TODO: Might not work
                H_S_S_SegmentView.Columns.RemoveAt(0);
                H_S_S_SegmentView.Columns.RemoveAt(0);
            //}

            DisplayVersionsForSegments();
        }

        private void DisplayVersionsForSegments()
        {
            DataTable versions = (DataTable)H_S_CBXVersionSelect.DataSource;
            H_S_ASCLBStudyVersion.Items.Clear();

            foreach (DataRow row in versions.Rows)
                H_S_ASCLBStudyVersion.Items.Add(row.ItemArray[0].ToString() + ": " + row.ItemArray[3].ToString());

        }

        private void H_S_ASBTNAddSegment_Click(object sender, EventArgs e)
        {
            if(H_S_ASLBLSegmentName.Text == "")
            {
                MessageBox.Show("Please fill the segment name field");
                return;
            }
            if(H_S_ASCLBStudyVersion.CheckedItems.Count <= 0)
            {
                MessageBox.Show("Please select at least 1 version to assign the segment too");
                return;
            }

            string segmentName = H_S_ASTXTSegmentName.Text;
            string segmentSource = H_S_ASTXTSegmentSource.Text;
            string dur = H_S_ASMTXTSegmentDuration.Text;
            DateTime duration;
            if(!DateTime.TryParse(dur, out duration))
            {
                duration = DateTime.Parse("00:00");
            }

            string sqlCommand = "INSERT INTO SEGMENT (VERSION_ID, NAME, DURATION, SOURCE) VALUES (@versID, @segName, @segDuration, @segSource)";

            // Iterate through each 
            for (int v = 0; v < H_S_ASCLBStudyVersion.CheckedItems.Count; v++)
            {
                // Gets the VERSION_ID of the version checked
                // "1: Adult" -> int 1
                int checkedVersionID;
                string version = H_S_ASCLBStudyVersion.CheckedItems[v].ToString();
                int.TryParse(version.Substring(0, version.IndexOf(":")), out checkedVersionID);

                SqlParameter[] parameters = new SqlParameter[4];
                parameters[0] = new SqlParameter("@versID", checkedVersionID);
                parameters[1] = new SqlParameter("@segName", segmentName);
                parameters[2] = new SqlParameter("@segDuration", duration);
                parameters[3] = new SqlParameter("@segSource", segmentSource);

                db.executeCommand(sqlCommand, parameters);
            }

            H_S_populateStudyVersions(selectedVersionID);
        }

        #endregion

        #region Participants

        private void LoadParticipants()
        {
            string query = "SELECT PARTICIPANT_ID, FIRST_NAME, LAST_NAME, AGE FROM PARTICIPANT";
            DataTable result = db.compileQuery(query);

            result.Columns.Add("displayCol", typeof(string), "FIRST_NAME + ' ' + LAST_NAME + ' : ' + AGE");

            H_P_CBXSelectParticipant.DataSource = result;
            H_P_CBXSelectParticipant.ValueMember = "PARTICIPANT_ID";
            H_P_CBXSelectParticipant.DisplayMember = "displayCol";
            
        }

        private List<object> fieldControls = new List<object>();

        private void H_P_CBXSelectParticipant_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Query for the data row of the chosen participant

            string query = "SELECT * FROM PARTICIPANT WHERE PARTICIPANT_ID = @parPARTICIPANT_ID";
            SqlParameter[] parameters = new SqlParameter[] {
            new SqlParameter("@parPARTICIPANT_ID", ((DataTable)H_P_CBXSelectParticipant.DataSource).Rows[H_P_CBXSelectParticipant.SelectedIndex].ItemArray[0])};

            DataTable result = db.compileQuery(query, parameters);
            Console.WriteLine("Participant row count: " + result.Rows.Count);
            if (result.Rows.Count <= 0)
                return;

            //H_P_DGVFields.DataSource = result;
            DataRow choosenParticipant = result.Rows[0];

            // Clear field control list
            fieldControls.Clear();
            H_P_FLPFields.Controls.Clear();

            // Loop through all fields, creating a label and a corresponding data entry control (Text, CBX, or Date Picker)
            for(int i = 0; i < choosenParticipant.ItemArray.Length; i++)
            {
                string fieldLabel = result.Columns[i].ColumnName;
                MetroFramework.Controls.MetroLabel fieldLabelControl = new MetroFramework.Controls.MetroLabel();
                fieldLabelControl.Text = fieldLabel;
                //H_P_FLPFields.Controls.Add(fieldLabelControl);

                // Check if the field is a special field (cbx or date)
                string data = choosenParticipant.ItemArray[i].ToString();

                TextBoxControl tbx = new TextBoxControl();
                tbx.id = i;
                tbx.lbl.Text = fieldLabel;
                tbx.txt.Text = data;

                fieldControls.Add(tbx);
                H_P_FLPFields.Controls.Add(tbx);
            }
        }


        #endregion

        #region Sessions

        private void LoadSessions()
        {

        }


        #endregion

        #region Locations

        private void LoadLocations()
        {

        }

        #endregion

        #endregion

        #region DataInput

        DatabaseEnums.TABLES currentTable;
        bool DI_ChangesMade = false;
        public void LoadDataInput()
        {
            //Populate table selector
            DI_CBXTableSelector.Items.Clear();
            for (int i = 0; i < (int)DatabaseEnums.TABLES.NUMBER_OF_COLS; i++)
            {
                DI_CBXTableSelector.Items.Add((DatabaseEnums.TABLES)i);
            }
        }

        /// <summary>
        /// Table selector changed, load new table
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DI_CBXTableSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check Changes
            if (!HandleDIChangesMade())
            {
                DI_CBXTableSelector.SelectedItem = currentTable;
                return;
            }

            currentTable = (DatabaseEnums.TABLES)DI_CBXTableSelector.SelectedIndex;
            selectNewTable(currentTable);
        }

        private void selectNewTable(DatabaseEnums.TABLES newTable)
        {
            // Select all from the chosen table
            DI_DGInput.Columns.Clear();
            DI_DGInput.DataSource = new DataTable();
            
            //enableSelectors(newTable);
            string query = "SELECT * FROM " + newTable;
            DataTable result = db.compileQuery(query);
            DI_DGInput.DataSource = result;
            populateDataGridView(DI_DGInput);
            //DI_DGInput.AutoGenerateColumns = false;
            //populateDataGridView(result);
            formatDataGrid(DI_DGInput);
            //addComboBoxesToDataGrid(result);
        }

        private bool HandleDIChangesMade()
        {
            if (DI_ChangesMade)
            {
                DialogResult result = MessageBox.Show("Data Input changes detected, do you wish to save", "Save Changes", MessageBoxButtons.YesNo);
                if(result == DialogResult.Yes)
                {
                    ((DataTable)DI_DGInput.DataSource).AcceptChanges();
                    SaveDataInput();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }


        /*private void enableSelectors(DatabaseEnums.TABLES table)
        {
            DI_CBXStudySelector.Enabled = true;
            DI_CBXParticipantSelector.Enabled = true;
            switch (table)
            {
                case DatabaseEnums.TABLES.STUDY:
                    {
                        DI_CBXStudySelector.Enabled = false;
                        DI_CBXParticipantSelector.Enabled = false;
                    }
                    break;
                case DatabaseEnums.TABLES.STUDY_VERSION:
                    {
                        DI_CBXParticipantSelector.Enabled = false;
                    }
                    break;
                case DatabaseEnums.TABLES.SEGMENT:
                    {
                        DI_CBXParticipantSelector.Enabled = false;
                    }
                    break;
                case DatabaseEnums.TABLES.SESSION:
                    {

                    }
                    break;
                case DatabaseEnums.TABLES.EEG_AMPLITUDE:
                    {

                    }
                    break;
                case DatabaseEnums.TABLES.PARTICIPANT:
                    {
                        DI_CBXStudySelector.Enabled = false;
                        DI_CBXParticipantSelector.Enabled = false;
                    }
                    break;
                case DatabaseEnums.TABLES.VISIT:
                    {
                        DI_CBXStudySelector.Enabled = false;
                        DI_CBXParticipantSelector.Enabled = false;
                    }
                    break;
                case DatabaseEnums.TABLES.LOCATION:
                    {
                        DI_CBXStudySelector.Enabled = false;
                        DI_CBXParticipantSelector.Enabled = false;
                    }
                    break;
            }
            string studyList = "SELECT * FROM STUDY";
            DataTable studyResult = db.compileQuery(studyList);
            DI_CBXStudySelector.DataSource = studyResult.DefaultView;
            DI_CBXStudySelector.DisplayMember = "NAME";
            DI_CBXStudySelector.ValueMember = "STUDY_ID";
            DI_CBXStudySelector.BindingContext = this.BindingContext;

            string partList = "SELECT * FROM PARTICIPANT";
            DataTable partResult = db.compileQuery(partList);
            partResult.Columns.Add("FullName",typeof(string),"FIRST_NAME + ' ' + LAST_NAME");

            DI_CBXParticipantSelector.DataSource = partResult.DefaultView;
            DI_CBXParticipantSelector.DisplayMember = "FullName";
            DI_CBXParticipantSelector.ValueMember = "PARTICIPANT_ID";
            DI_CBXParticipantSelector.BindingContext = this.BindingContext;

        }*/

        public void formatDataGrid(DataGridView grid)
        {
            for(int c = 0; c < grid.Columns.Count; c++)
            {
                if(ColumnIsForeignKey(grid.Columns[c]))
                {
                    grid.Columns[c].ReadOnly = true;
                    grid.Columns[c].DefaultCellStyle.BackColor = Color.LightGray;
                }
                else if (grid.Columns[c].ReadOnly)
                {
                    grid.Columns[c].DefaultCellStyle.BackColor = Color.Gray;
                }
            }
        }

        private bool ColumnIsForeignKey(DataGridViewColumn column)
        {
            return DatabaseHandler.getValidValuesForForiegnKey(column.Name, currentTable.ToString()) != "";
        }

        /// <summary>
        /// Adds the ComboBox Columns to replace foreign key values
        /// </summary>
        /// <param name="grid"></param>
        public void populateDataGridView(DataGridView grid)
        {
            DataTable table = (DataTable)grid.DataSource;
            // Look through each column
            for(int i = 0; i < table.Columns.Count; i++)
            {
                var c = table.Columns[i];
                // col is read only which means it needs supplied values
                string query = DatabaseHandler.getValidValuesForForiegnKey(c.ColumnName, currentTable.ToString());
                //Console.WriteLine("Query: " + query);
                if (query != "")
                {
                    Console.WriteLine("Replacing column: " + c.ColumnName + " with a combo box");

                    DataTable result = new DataTable();
                    try
                    {
                        Console.WriteLine("Query: " + query);
                        result = db.compileQuery(query);

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }

                    // Display field will be all values but the foreign key
                    string dataFields = "";
                    for(int remainingFields = 1; remainingFields < result.Columns.Count; remainingFields++)
                    {
                        dataFields += result.Columns[remainingFields];
                        if (remainingFields < result.Columns.Count - 1)
                            dataFields += " + ' ' + ";
                    }
                    
                    // Column name is the foreign key trimmed from the '_ID'
                    string colName = c.ColumnName;
                    colName = colName.Split('_')[0];
                    result.Columns.Add(colName, typeof(string), dataFields);

                    var comboCol = new DataGridViewComboBoxColumn();
                    comboCol.DataSource = result;
                    comboCol.DisplayMember = colName;
                    comboCol.Name = colName;
                    comboCol.DataPropertyName = result.Columns[0].ColumnName;
                    comboCol.ValueMember = result.Columns[0].ColumnName;
                    comboCol.AutoComplete = true;
                    comboCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    
                    grid.Columns.RemoveAt(i);
                    grid.Columns.Insert(i,comboCol);
                    
                    // Loop over each row of the new column, setting the appropriate value   
                    //for(int r = 0; r < grid.Rows.Count; r++)
                    //{
                    //    grid.Rows[r].Cells[i].Value = result.
                    //}
                    
                }
                else
                {
                    //var newCol = new Data
                }
            }
            //DI_DGInput = grid;
        }

        public void MergePasteData(DataGridView pasteData, DatabaseEnums.TABLES pasteTable)
        {
            if(currentTable != pasteTable)
            {
                selectNewTable(pasteTable);
            }
            DataTable dataToAdd = (DataTable)pasteData.DataSource;

            Console.WriteLine("Merging tables: \nTable to Paste: ");
            printDataTable(dataToAdd);
            
            Console.WriteLine("Current Table");
            printDataTable((DataTable)DI_DGInput.DataSource);

            DataTable t = (DataTable)DI_DGInput.DataSource;
            t.Merge(dataToAdd);
            DI_DGInput.DataSource = t;
            Console.WriteLine("Result Table");
            printDataTable((DataTable)DI_DGInput.DataSource);

            SaveDataInput();
            this.Focus();
            DI_DGInput.Refresh();
        }

        private void DI_BTNSave_Click(object sender, EventArgs e)
        {
            SaveDataInput();
        }

       
        private void DI_BTNPasteFromClipboard_Click(object sender, EventArgs e)
        {
            PasteForm pf = new PasteForm();
            pf.setParams(this, (DataTable)DI_DGInput.DataSource, db, currentTable);
            pf.Show();
            pf.Focus();
        }

        private void DI_BTNDeleteRow_Click_1(object sender, EventArgs e)
        {
            if (DI_DGInput.SelectedRows.Count == 0)
                return;

            string rowString = "\n";
            for(int c = 0; c < DI_DGInput.SelectedRows[0].Cells.Count; c++)
            {
                rowString += DI_DGInput.SelectedRows[0].Cells[c].OwningColumn.Name + ":\t" + DI_DGInput.SelectedRows[0].Cells[c].Value + "\n";
            }
            if(MessageBox.Show(("Are you sure you wish to delete (" + rowString + ")"),"Confirm Delete", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            // Prepare the command text with the parameter placeholder
            string sql = "DELETE FROM " + currentTable.ToString() + " WHERE ";
            
            DataTable pkTable = db.getPrimaryKeyFromTable(currentTable);


            if (pkTable.Rows.Count == 0)
            {
                // Row not formed? just delete from datagrid
                DI_DGInput.Rows.RemoveAt(DI_DGInput.SelectedRows[0].Index);
                return;
            }

            SqlParameter[] par = new SqlParameter[pkTable.Rows.Count];
            for (int i = 0; i < pkTable.Rows.Count; i++)
            {
                if (i != 0)
                    sql += " OR ";
                sql += pkTable.Rows[i][1] + " = @row" + pkTable.Rows[i][1];
                // Create sqlParams
                string colName = (string)pkTable.Rows[i][1];
                int colIndex = 0;
                for(int c = 0; c < DI_DGInput.Columns.Count; c++)
                {
                    if (DI_DGInput.Columns[c].Name == colName)
                    {
                        colIndex = c;
                        break;
                    }
                }
                par[i] = new SqlParameter("@row" + colName, SqlDbType.Int);
                par[i].Value = (int)DI_DGInput.SelectedRows[0].Cells[colIndex].Value;
            }

            try
            {
                db.executeCommand(sql, par);
            }
            catch(SqlException ex)
            {
                MessageBox.Show("Child values linked to this row, please delete linked rows before deleteing this row","Delete Row Failed");
                Console.Write(ex.Message + "\n" + ex.StackTrace);
            }

            // Remove the row from the grid
            DI_DGInput.Rows.RemoveAt(DI_DGInput.SelectedRows[0].Index);

            selectNewTable(currentTable);

            refactorTable(currentTable, pkTable.Rows[0][1].ToString());
        }

        private void refactorTable(DatabaseEnums.TABLES table, string col)
        {
            string refactorCommand = @"declare @max int
                                    select @max = max([" + col + @"]) from[" + table + @"]
                                    if @max IS NULL   --check when max is returned as null
                                    SET @max = 0
                                    DBCC CHECKIDENT('[" + table + @"]', RESEED, @max)";

            db.executeCommand(refactorCommand);
        }

        public void SaveDataInput()
        {
            try
            {
                //db.copyDataTableToDatabase((DataTable)DI_DGInput.DataSource, currentTable);
                int rowsChanged = db.mergeDataTable((DataTable)DI_DGInput.DataSource, currentTable);
                selectNewTable(currentTable);
                DI_ChangesMade = false;
                MessageBox.Show(rowsChanged.ToString() + " Changed", "Merge Executed");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DI_DGInput_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            DI_ChangesMade = true;
        }

        

        #endregion

        #region SQLScript

        private void SQL_BTNRunScript_Click(object sender, EventArgs e)
        {
            try
            {
                if (SQL_TXTScript.Lines.Length > 0)
                {
                    foreach (string script in SQL_TXTScript.Lines)
                    {
                        db.executeCommand(script);
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SQL_BTNRunQuery_Click(object sender, EventArgs e)
        {
            try
            {
                if (SQL_TXTScript.Lines.Length > 0)
                {
                    string script = SQL_TXTScript.Lines[0];
                    DataTable result = db.compileQuery(script);
                    SQL_DGOutput.DataSource = result;

                    for(int i = 0; i < SQL_TXTScript.Lines.Length-1; i++)
                    {
                        SQL_TXTScript.Lines[i] = SQL_TXTScript.Lines[i + 1];
                    }
                    SQL_TXTScript.Lines[SQL_TXTScript.Lines.Length - 1] = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }




        #endregion

        #region QueryData

        /// <summary>
        /// Loads the Query Data tab and populates the prompt cbx
        /// </summary>
        private void LoadQueryData()
        {
            // Populate promts cbx with prompts
            var bindingSource = new BindingSource();
            bindingSource.DataSource = ph.prompts;
            QD_CBXChoosePrompt.DataSource = bindingSource.DataSource;
            QD_CBXChoosePrompt.DisplayMember = "prompt";
            QD_CBXChoosePrompt.ValueMember = "prompt";

            Console.WriteLine("Prompts added to Prompt CBX");
            //foreach(Prompt p in (List<Prompt>)QD_CBXChoosePrompt.DataSource)
            //{
            //    Console.WriteLine(p.ToString());
            //}
            
        }

        private void QD_CBXChoosePrompt_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Big boi method time
            // Get choosen prompt
            Prompt choosenPrompt = (Prompt)QD_CBXChoosePrompt.SelectedItem;
            choosenPrompt.rank++;

            QD_PopulateFLP(choosenPrompt);

            
        }

        private List<int> optionIndexes;

        /// <summary>
        /// Fill the FLP with Labels and ComboBoxControls corresponding to the prompt
        /// </summary>
        /// <param name="p"></param>
        private void QD_PopulateFLP(Prompt p)
        {
            // Get all none-Option parts of the prompt, and _ for options
            string[] sections = p.prompt.Split(new string[] {" _","_ "}, StringSplitOptions.RemoveEmptyEntries);


            // Clear FLP
            QD_FLP.Controls.Clear();
            optionIndexes = new List<int>();
            int optionCounter = 0;

            Console.WriteLine("Prompt Sections:");
            for (int s = 0; s < sections.Length; s++)
            {
                Console.WriteLine(sections[s]);
                if(sections[s] == "_")
                {
                    // Option
                    var cbx = new MetroFramework.Controls.MetroComboBox();
                    cbx.Style = MetroFramework.MetroColorStyle.Magenta;
                    cbx.Size = new Size(300, cbx.Size.Height);
                    cbx.DropDownWidth = 300;

                    // Query for table
                    try
                    {
                        // Get corresponding string for query from the prompt
                        DataTable optionResult = db.compileQuery(p.options[optionCounter]);
                        printDataTable(optionResult);
                        cbx.DataSource = optionResult;
                        cbx.ValueMember = optionResult.Columns[0].ColumnName;

                        // Display field will be all values but the primary key
                        string dataFields = "";
                        for (int remainingFields = 1; remainingFields < optionResult.Columns.Count; remainingFields++)
                        {
                            dataFields += optionResult.Columns[remainingFields];
                            if (remainingFields < optionResult.Columns.Count - 1)
                                dataFields += " + ' ' + ";
                        }

                        // Create Column with new data
                        string colName = "Display Column";
                        optionResult.Columns.Add(colName, typeof(string), dataFields);

                        cbx.DisplayMember = colName;
                        if (cbx.Items.Count > 0)
                            cbx.SelectedIndex = 0;

                        
                    }
                    catch(SqlException ex)
                    {
                        Console.WriteLine("Prompt Option query failed: \n" + p.options[optionCounter] + "\n" + ex.Message);
                    }

                    optionIndexes.Add(QD_FLP.Controls.Count);
                    QD_FLP.Controls.Add(cbx);
                    optionCounter++;
                }
                else
                {
                    // Text
                    var label = new MetroFramework.Controls.MetroLabel();
                    label.Style = MetroFramework.MetroColorStyle.Magenta;
                    label.Text = sections[s];
                    label.AutoSize = true;
                    QD_FLP.Controls.Add(label);
                }
            }

        }

        private void QD_BTNQuery_Click(object sender, EventArgs e)
        {
            // Get choosen prompt
            Prompt choosenPrompt = (Prompt)QD_CBXChoosePrompt.SelectedItem;

            QD_CompilePromptQuery(choosenPrompt);

            ph.prompts.Sort();
            ph.writePrompts();
        }

        private void QD_CompilePromptQuery(Prompt p)
        {
            Console.WriteLine("Compiling Prompt Query");
            // Translate prompt Query
            string query = p.query;
            query = query.Replace("(AllTables)", DatabaseHandler.JoinStudyKey);

            // Get Option Values
            if (optionIndexes.Count != p.options.Length)
            {
                Console.WriteLine("Option Length Mismatch! " + optionIndexes.Count + " != " + p.options.Length);
                return;
            }

            for(int i = 0; i < optionIndexes.Count; i++)
            {
                var optionCBX = ((MetroFramework.Controls.MetroComboBox)QD_FLP.Controls[optionIndexes[i]]);
                if (optionCBX.SelectedIndex == -1)
                    optionCBX.SelectedIndex = 0;

                // Should replace {0} with first value of cbx
                query = query.Replace("{" + i + "}", ((DataRowView)optionCBX.SelectedItem).Row.ItemArray[0].ToString());
            }
            // Compile final Query
            Console.WriteLine("Final Query: " + query);
            DataTable result = db.compileQuery(query);
            printDataTable(result);
            // Show Result
            QD_DGResult.DataSource = result;
        }

        #endregion

        #region Utility

        public void printDataTable(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                Console.WriteLine("Row: ");
                foreach (object col in row.ItemArray)
                {
                    Console.Write("\t" + col.ToString());
                }
            }
            Console.Write("\n");
        }



        #endregion

        
    }
}
