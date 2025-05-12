namespace Farmacia_Presentacion
{
    partial class Form_BusquedaFactura
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_BusquedaFactura));
            this.textBox_Factura = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_Buscar = new System.Windows.Forms.Button();
            this.rtb_Factura = new System.Windows.Forms.RichTextBox();
            this.btn_Guardar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox_Factura
            // 
            this.textBox_Factura.Location = new System.Drawing.Point(261, 7);
            this.textBox_Factura.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBox_Factura.Name = "textBox_Factura";
            this.textBox_Factura.Size = new System.Drawing.Size(223, 22);
            this.textBox_Factura.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 16);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(184, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Ingresar Codigo de la Factura";
            // 
            // button_Buscar
            // 
            this.button_Buscar.Location = new System.Drawing.Point(533, 6);
            this.button_Buscar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.button_Buscar.Name = "button_Buscar";
            this.button_Buscar.Size = new System.Drawing.Size(100, 28);
            this.button_Buscar.TabIndex = 11;
            this.button_Buscar.Text = "Buscar";
            this.button_Buscar.UseVisualStyleBackColor = true;
            this.button_Buscar.Click += new System.EventHandler(this.button_Buscar_Click);
            // 
            // rtb_Factura
            // 
            this.rtb_Factura.Location = new System.Drawing.Point(39, 59);
            this.rtb_Factura.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rtb_Factura.Name = "rtb_Factura";
            this.rtb_Factura.Size = new System.Drawing.Size(746, 645);
            this.rtb_Factura.TabIndex = 12;
            this.rtb_Factura.Text = "";
            // 
            // btn_Guardar
            // 
            this.btn_Guardar.Location = new System.Drawing.Point(687, 6);
            this.btn_Guardar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btn_Guardar.Name = "btn_Guardar";
            this.btn_Guardar.Size = new System.Drawing.Size(100, 28);
            this.btn_Guardar.TabIndex = 13;
            this.btn_Guardar.Text = "Guardar";
            this.btn_Guardar.UseVisualStyleBackColor = true;
            this.btn_Guardar.Click += new System.EventHandler(this.btn_Guardar_Click);
            // 
            // Form_BusquedaFactura
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.ClientSize = new System.Drawing.Size(803, 720);
            this.Controls.Add(this.btn_Guardar);
            this.Controls.Add(this.rtb_Factura);
            this.Controls.Add(this.button_Buscar);
            this.Controls.Add(this.textBox_Factura);
            this.Controls.Add(this.label3);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Form_BusquedaFactura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Buscar Factura";
            this.Load += new System.EventHandler(this.Form_BusquedaFactura_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Factura;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Buscar;
        private System.Windows.Forms.RichTextBox rtb_Factura;
        private System.Windows.Forms.Button btn_Guardar;
    }
}