namespace Castle.ActiveWriter.UIEditors
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Text;
	using System.Windows.Forms;
	using ARValidators;

	// Had some problems with CollectionEditor. I'm writing my own. Ugly, but works for now.
	public partial class PropertyValidationEditorForm : Form
	{
		private ArrayList list;

		public PropertyValidationEditorForm(ArrayList list)
		{
			this.list = list;

			InitializeComponent();

			if (this.list != null)
			{
				validatorList.DataSource = this.list;
			}

			errors.SetIconAlignment(okButton, ErrorIconAlignment.MiddleLeft);
		}

		public ArrayList Value
		{
			get
			{
				// Return null on empty. Otherwise empty arraylist will eventually serialized and
				// confuse the code generator to generate the code derived from ARValidatorBase rather than ARBase.
				if (list != null && list.Count == 0)
				{
					return null;
				}

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

		private void collectionNotEmptyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateCollectionNotEmpty());
		}

		private void dateToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateDate());
		}

		private void dateTimeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateDateTime());
		}

		private void decimalToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateDecimal());
		}

		private void doubleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateDouble());
		}

		private void groupNotEmptyToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateGroupNotEmpty());
		}

		private void integerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateInteger());
		}

		private void rangeToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateRange());
		}

		private void sameAsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateSameAs());
		}

		private void setToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateSet());
		}

		private void singleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Add(new ValidateSingle());
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
				{
					propertyDisplay.SelectedObject = list[validatorList.SelectedIndex];
				}
			}
		}

		private void RefreshListBox()
		{
			CurrencyManager cm = (CurrencyManager) this.BindingContext[list];
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

				foreach(AbstractValidation validation in list)
				{
					validation.IsValid(errorList);
				}

				if (errorList.Count > 0)
				{
					StringBuilder builder = new StringBuilder();
					foreach(string s in errorList)
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