using System.Text;

namespace Altinoren.ActiveWriter.UIEditors
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Altinoren.ActiveWriter.ARValidators;

    // Had some problems with CollectionEditor. I'm writing my own. Ugly, but works for now.
    public partial class PropertyValidationEditorForm : Form
    {
        private ArrayList list;

        public PropertyValidationEditorForm(ArrayList list)
        {
            this.list = list;

            InitializeComponent();

            if (this.list != null)
                validatorList.DataSource = this.list;

            errors.SetIconAlignment(okButton, ErrorIconAlignment.MiddleLeft);
        }

        public ArrayList Value
        {
            get
            {
                // Return null on empty. Otherwise empty arraylist will eventually serialized and
                // confuse the code generator to generate the code derived from ARValidatorBase rather than ARBase.
                if (list != null && list.Count == 0)
                    return null;

                return list;
            }
        }

        private void validatorList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (validatorList.SelectedIndex < 0)
            {
                removeButton.Enabled = false;
                propertyDisplay.SelectedObject = null;
            }
            else
            {
                removeButton.Enabled = true;
                propertyDisplay.SelectedObject = list[validatorList.SelectedIndex];
            }
        }

        private void Add(object o)
        {
            if (list == null)
            {
                list = new ArrayList();
                validatorList.DataSource = list;
            }
            list.Add(o);

            RefreshListBox();

            validatorList.SelectedIndex = list.Count - 1;
        }


        private void creditCardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add(new ValidateCreditCard());
        }

        private void emailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add(new ValidateEmail());
        }

        private void lengthToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add(new ValidateLength());
        }

        private void notEmptyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add(new ValidateNonEmpty());
        }

        private void regularExpressionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Add(new ValidateRegExp());
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (validatorList.SelectedIndex >= 0)
            {
                list.RemoveAt(validatorList.SelectedIndex);

                RefreshListBox();

                if (validatorList.Items.Count == 0)
                {
                    propertyDisplay.SelectedObject = null;
                    removeButton.Enabled = false;
                }
                else
                    propertyDisplay.SelectedObject = list[validatorList.SelectedIndex];
            }
        }

        private void RefreshListBox()
        {
            CurrencyManager cm = (CurrencyManager)this.BindingContext[list];
            cm.Refresh();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (IsValid())
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool IsValid()
        {
            if (list != null && list.Count > 0)
            {
                List<string> errorList = new List<string>();

                foreach (AbstractValidation validation in list)
                {
                    validation.IsValid(errorList);
                }

                if (errorList.Count > 0)
                {
                    StringBuilder builder = new StringBuilder();
                    foreach (string s in errorList)
                    {
                        builder.Append(s);
                        builder.Append("\n");
                    }
                    errors.SetError(okButton, builder.ToString());
                    return false;
                }
            }

            return true;
        }
    }
}