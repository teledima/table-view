namespace lab6
{
    partial class FormPresentantion
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.data_grid_view = new System.Windows.Forms.DataGridView();
            this.button_delete = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.data_grid_view)).BeginInit();
            this.SuspendLayout();
            // 
            // data_grid_view
            // 
            this.data_grid_view.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.data_grid_view.Location = new System.Drawing.Point(12, 7);
            this.data_grid_view.Name = "data_grid_view";
            this.data_grid_view.RowHeadersWidth = 51;
            this.data_grid_view.RowTemplate.Height = 24;
            this.data_grid_view.Size = new System.Drawing.Size(923, 504);
            this.data_grid_view.TabIndex = 0;
            this.data_grid_view.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.data_grid_view_RowValidating);
            // 
            // button_delete
            // 
            this.button_delete.Location = new System.Drawing.Point(12, 517);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(290, 53);
            this.button_delete.TabIndex = 5;
            this.button_delete.Text = "Delete";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // FormPresentantion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1130, 599);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.data_grid_view);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormPresentantion";
            this.Text = "FormDgvIud";
            ((System.ComponentModel.ISupportInitialize)(this.data_grid_view)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView data_grid_view;
        private System.Windows.Forms.Button button_delete;
    }
}

