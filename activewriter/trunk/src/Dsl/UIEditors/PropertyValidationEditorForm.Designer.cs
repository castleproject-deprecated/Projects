namespace Altinoren.ActiveWriter.UIEditors
{
    partial class PropertyValidationEditorForm
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
            this.components = new System.ComponentModel.Container();
            this.validatorList = new System.Windows.Forms.ListBox();
            this.propertyDisplay = new System.Windows.Forms.PropertyGrid();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.addButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.creditCardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.emailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lengthToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.notEmptyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.regularExpressionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.removeButton = new System.Windows.Forms.ToolStripButton();
            this.errors = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).BeginInit();
            this.SuspendLayout();
            // 
            // validatorList
            // 
            this.validatorList.FormattingEnabled = true;
            this.validatorList.Location = new System.Drawing.Point(4, 30);
            this.validatorList.Name = "validatorList";
            this.validatorList.Size = new System.Drawing.Size(154, 238);
            this.validatorList.TabIndex = 4;
            this.validatorList.SelectedIndexChanged += new System.EventHandler(this.validatorList_SelectedIndexChanged);
            // 
            // propertyDisplay
            // 
            this.propertyDisplay.Location = new System.Drawing.Point(164, 5);
            this.propertyDisplay.Name = "propertyDisplay";
            this.propertyDisplay.Size = new System.Drawing.Size(284, 263);
            this.propertyDisplay.TabIndex = 5;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(292, 277);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 7;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(373, 277);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 8;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AutoSize = false;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addButton,
            this.removeButton});
            this.toolStrip1.Location = new System.Drawing.Point(4, 6);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(154, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // addButton
            // 
            this.addButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.addButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.creditCardToolStripMenuItem,
            this.emailToolStripMenuItem,
            this.lengthToolStripMenuItem,
            this.notEmptyToolStripMenuItem,
            this.regularExpressionToolStripMenuItem});
            this.addButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(51, 22);
            this.addButton.Text = "Add...";
            // 
            // creditCardToolStripMenuItem
            // 
            this.creditCardToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.creditCardToolStripMenuItem.Name = "creditCardToolStripMenuItem";
            this.creditCardToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.creditCardToolStripMenuItem.Text = "Credit Card";
            this.creditCardToolStripMenuItem.Click += new System.EventHandler(this.creditCardToolStripMenuItem_Click);
            // 
            // emailToolStripMenuItem
            // 
            this.emailToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.emailToolStripMenuItem.Name = "emailToolStripMenuItem";
            this.emailToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.emailToolStripMenuItem.Text = "Email";
            this.emailToolStripMenuItem.Click += new System.EventHandler(this.emailToolStripMenuItem_Click);
            // 
            // lengthToolStripMenuItem
            // 
            this.lengthToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lengthToolStripMenuItem.Name = "lengthToolStripMenuItem";
            this.lengthToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.lengthToolStripMenuItem.Text = "Length";
            this.lengthToolStripMenuItem.Click += new System.EventHandler(this.lengthToolStripMenuItem_Click);
            // 
            // notEmptyToolStripMenuItem
            // 
            this.notEmptyToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.notEmptyToolStripMenuItem.Name = "notEmptyToolStripMenuItem";
            this.notEmptyToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.notEmptyToolStripMenuItem.Text = "Non Empty";
            this.notEmptyToolStripMenuItem.Click += new System.EventHandler(this.notEmptyToolStripMenuItem_Click);
            // 
            // regularExpressionToolStripMenuItem
            // 
            this.regularExpressionToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.regularExpressionToolStripMenuItem.Name = "regularExpressionToolStripMenuItem";
            this.regularExpressionToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.regularExpressionToolStripMenuItem.Text = "Regular Expression";
            this.regularExpressionToolStripMenuItem.Click += new System.EventHandler(this.regularExpressionToolStripMenuItem_Click);
            // 
            // removeButton
            // 
            this.removeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.removeButton.Enabled = false;
            this.removeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(50, 22);
            this.removeButton.Text = "Remove";
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // errors
            // 
            this.errors.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errors.ContainerControl = this;
            // 
            // PropertyValidationEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(454, 307);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.propertyDisplay);
            this.Controls.Add(this.validatorList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertyValidationEditorForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Edit Validations";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errors)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox validatorList;
        private System.Windows.Forms.PropertyGrid propertyDisplay;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripDropDownButton addButton;
        private System.Windows.Forms.ToolStripButton removeButton;
        private System.Windows.Forms.ToolStripMenuItem creditCardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem emailToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lengthToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem notEmptyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem regularExpressionToolStripMenuItem;
        private System.Windows.Forms.ErrorProvider errors;
    }
}