namespace Farmacia_Presentacion
{
    partial class Form_Producto
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Producto));
            this.dataGridView_Productos = new System.Windows.Forms.DataGridView();
            this.textBox_Nombre = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Productos)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_Productos
            // 
            this.dataGridView_Productos.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Productos.Location = new System.Drawing.Point(99, 77);
            this.dataGridView_Productos.Name = "dataGridView_Productos";
            this.dataGridView_Productos.Size = new System.Drawing.Size(694, 407);
            this.dataGridView_Productos.TabIndex = 0;
            this.dataGridView_Productos.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_Productos_CellClick);
            // 
            // textBox_Nombre
            // 
            this.textBox_Nombre.Location = new System.Drawing.Point(252, 45);
            this.textBox_Nombre.Name = "textBox_Nombre";
            this.textBox_Nombre.Size = new System.Drawing.Size(100, 20);
            this.textBox_Nombre.TabIndex = 9;
            this.textBox_Nombre.TextChanged += new System.EventHandler(this.textBox_Nombre_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "NOMBRE";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(391, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "CRITERIOS DE BUSQUEDAS";
            // 
            // Form_Producto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(890, 514);
            this.Controls.Add(this.textBox_Nombre);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView_Productos);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form_Producto";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SELECCIONE UN PRODUCTO";
            this.Load += new System.EventHandler(this.Form_Producto_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Productos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_Productos;
        private System.Windows.Forms.TextBox textBox_Nombre;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}