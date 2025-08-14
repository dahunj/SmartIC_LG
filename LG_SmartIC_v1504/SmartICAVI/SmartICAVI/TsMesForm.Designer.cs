namespace SmartICAVI
{
    partial class TsMesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TsMesForm));
            this.axucTibcoRv1 = new AxaxTibcoRv.AxucTibcoRv();
            ((System.ComponentModel.ISupportInitialize)(this.axucTibcoRv1)).BeginInit();
            this.SuspendLayout();
            // 
            // axucTibcoRv1
            // 
            this.axucTibcoRv1.Enabled = true;
            this.axucTibcoRv1.Location = new System.Drawing.Point(12, 12);
            this.axucTibcoRv1.Name = "axucTibcoRv1";
            this.axucTibcoRv1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axucTibcoRv1.OcxState")));
            this.axucTibcoRv1.Size = new System.Drawing.Size(412, 511);
            this.axucTibcoRv1.TabIndex = 0;
            // 
            // TsMesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(440, 531);
            this.Controls.Add(this.axucTibcoRv1);
            this.Name = "TsMesForm";
            this.Text = "TsMesForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TsMesForm_FormClosed);
            this.Load += new System.EventHandler(this.TsMesForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.axucTibcoRv1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private AxaxTibcoRv.AxucTibcoRv axucTibcoRv1;
    }
}