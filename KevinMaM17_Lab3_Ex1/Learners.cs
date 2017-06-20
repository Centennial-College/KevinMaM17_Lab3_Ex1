using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using KevinDBModel;
using System.Data.Entity;

namespace KevinMaM17_Lab3_Ex1
{
    public partial class Learners : Form
    {
        public Learners()
        {
            InitializeComponent();
        }

        //Entity Framework DbContext
        private KevinDBEntities dbContext = null;

        //load data from database into the form
        private void Learners_Load(object sender, EventArgs e)
        {
            this._refreshContacts();
        }

        /// <summary>
        /// Fills the kevinTBBindingSource with all rows, ordered by learnerID
        /// </summary>
        private void _refreshContacts()
        {
            //dispose of old dbContext, if any
            if (dbContext != null)
            {
                dbContext.Dispose();
            }

            // create new DbContext so we can reorder records based on edits
            dbContext = new KevinDBEntities();

            //enables the save button
            kevinTBBindingNavigatorSaveItem.Enabled = true;

            //load KevinTB table ordered by learnerID
            dbContext.KevinTBs
                .OrderBy(learner => learner.learnerID)
                .Load();

            //specify DataSource for kevinTBBindingSource
            kevinTBBindingSource.DataSource = dbContext.KevinTBs.Local;
            kevinTBBindingSource.MoveFirst(); // go to first result     
        }

        private void kevinTBBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            Validate(); //validate the input fields
            kevinTBBindingSource.EndEdit();

            //try to save the changes
            try
            {
                dbContext.SaveChanges();//write changes to underlying db
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException)
            {
                MessageBox.Show("Learner ID, Learner Name and Enrolled Programs must contain values", "Entity Validation Exception");
            }

            this._refreshContacts();
        }

        private void searchButton_Click(object sender, EventArgs e)
        {
            // use LINQ to filter learners with enrolled program as specified
            var searchLearnerQuery =
               from learner in dbContext.KevinTBs
               where learner.enrolledProgram.Equals(searchTextBox.Text)
               orderby learner.learnerID
               select learner;

            // display matching contacts
            kevinTBBindingSource.DataSource = searchLearnerQuery.ToList();
            kevinTBBindingSource.MoveFirst(); // go to first result  

            // don't allow add/delete when contacts are filtered
            bindingNavigatorAddNewItem.Enabled = false;
            bindingNavigatorDeleteItem.Enabled = false;
        }
    }
}
