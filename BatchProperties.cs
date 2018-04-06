using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Data.OleDb;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CamGenie
{
    public partial class BatchProperties : Form
    {
        public string BatchName = "";
        public string BatchPath = "";
        public string BatchType = "PhoneBill";

        public BatchProperties()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            string batchConfigFile = BatchPath + "\\batch.config", batchTempFile = BatchPath + "\\batch.tmp";
            string line, fieldName, headerNumber;
            TextBox textBox;
            Hashtable hashtableHeaders = new Hashtable();
            StreamWriter sw = new StreamWriter(batchTempFile, false);

            if (File.Exists(batchConfigFile))
            {
                using (StreamReader sr = new StreamReader(batchConfigFile))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("Acctno") || line.StartsWith("Year") || line.StartsWith("Callfrom") || line.StartsWith("PageStart") || line.StartsWith("PageEnd"))
                        {
                            fieldName = line.Split('=')[0];
                            headerNumber = fieldName.Substring(fieldName.Length - 1, 1);
                            headerNumber = headerNumber == "0" ? "10" : headerNumber;
                            if (hashtableHeaders.ContainsKey("1") == false) hashtableHeaders.Add(headerNumber, headerNumber);
                            textBox = this.Controls.Find("textBox" + fieldName, true)[0] as TextBox;
                            
                            if (textBox.Visible == true)
                            {
                                sw.WriteLine(fieldName + "=" + textBox.Text);
                            }
                        }
                        else
                            sw.WriteLine(line);
                    }
                }

                File.Delete(batchConfigFile);
            }

            if (radioButtonPhoneBill.Checked == true)
                BatchType = "PhoneBill";
            else
                BatchType = "BankStatement";                

            sw.WriteLine("BatchType=" + BatchType);

            if (textBoxAcctno1.Visible == true)
            {
                sw.WriteLine("Acctno1=" + textBoxAcctno1.Text);
                sw.WriteLine("Year1=" + textBoxYear1.Text);
                sw.WriteLine("Callfrom1=" + textBoxCallfrom1.Text);
                sw.WriteLine("PageStart1=" + textBoxPageStart1.Text);
                sw.WriteLine("PageEnd1=" + textBoxPageEnd1.Text);
            }
            if (textBoxAcctno2.Visible == true && !hashtableHeaders.ContainsKey("2"))
            {
                sw.WriteLine("Acctno2=" + textBoxAcctno2.Text);
                sw.WriteLine("Year2=" + textBoxYear2.Text);
                sw.WriteLine("Callfrom2=" + textBoxCallfrom2.Text);
                sw.WriteLine("PageStart2=" + textBoxPageStart2.Text);
                sw.WriteLine("PageEnd2=" + textBoxPageEnd2.Text);
            }
            if (textBoxAcctno3.Visible == true && !hashtableHeaders.ContainsKey("3"))
            {
                sw.WriteLine("Acctno3=" + textBoxAcctno3.Text);
                sw.WriteLine("Year3=" + textBoxYear3.Text);
                sw.WriteLine("Callfrom3=" + textBoxCallfrom3.Text);
                sw.WriteLine("PageStart3=" + textBoxPageStart3.Text);
                sw.WriteLine("PageEnd3=" + textBoxPageEnd3.Text);
            }
            if (textBoxAcctno4.Visible == true && !hashtableHeaders.ContainsKey("4"))
            {
                sw.WriteLine("Acctno4=" + textBoxAcctno4.Text);
                sw.WriteLine("Year4=" + textBoxYear4.Text);
                sw.WriteLine("Callfrom4=" + textBoxCallfrom4.Text);
                sw.WriteLine("PageStart4=" + textBoxPageStart4.Text);
                sw.WriteLine("PageEnd4=" + textBoxPageEnd4.Text);
            }
            if (textBoxAcctno5.Visible == true && !hashtableHeaders.ContainsKey("5"))
            {
                sw.WriteLine("Acctno5=" + textBoxAcctno5.Text);
                sw.WriteLine("Year5=" + textBoxYear5.Text);
                sw.WriteLine("Callfrom5=" + textBoxCallfrom5.Text);
                sw.WriteLine("PageStart5=" + textBoxPageStart5.Text);
                sw.WriteLine("PageEnd5=" + textBoxPageEnd5.Text);
            }
            if (textBoxAcctno6.Visible == true && !hashtableHeaders.ContainsKey("6"))
            {
                sw.WriteLine("Acctno6=" + textBoxAcctno6.Text);
                sw.WriteLine("Year6=" + textBoxYear6.Text);
                sw.WriteLine("Callfrom6=" + textBoxCallfrom6.Text);
                sw.WriteLine("PageStart6=" + textBoxPageStart6.Text);
                sw.WriteLine("PageEnd6=" + textBoxPageEnd6.Text);
            }
            if (textBoxAcctno7.Visible == true && !hashtableHeaders.ContainsKey("7"))
            {
                sw.WriteLine("Acctno7=" + textBoxAcctno7.Text);
                sw.WriteLine("Year7=" + textBoxYear7.Text);
                sw.WriteLine("Callfrom7=" + textBoxCallfrom7.Text);
                sw.WriteLine("PageStart7=" + textBoxPageStart7.Text);
                sw.WriteLine("PageEnd7=" + textBoxPageEnd7.Text);
            }
            if (textBoxAcctno8.Visible == true && !hashtableHeaders.ContainsKey("8"))
            {
                sw.WriteLine("Acctno8=" + textBoxAcctno8.Text);
                sw.WriteLine("Year8=" + textBoxYear8.Text);
                sw.WriteLine("Callfrom8=" + textBoxCallfrom8.Text);
                sw.WriteLine("PageStart8=" + textBoxPageStart8.Text);
                sw.WriteLine("PageEnd8=" + textBoxPageEnd8.Text);
            }
            if (textBoxAcctno9.Visible == true && !hashtableHeaders.ContainsKey("9"))
            {
                sw.WriteLine("Acctno9=" + textBoxAcctno9.Text);
                sw.WriteLine("Year9=" + textBoxYear9.Text);
                sw.WriteLine("Callfrom9=" + textBoxCallfrom9.Text);
                sw.WriteLine("PageStart9=" + textBoxPageStart9.Text);
                sw.WriteLine("PageEnd9=" + textBoxPageEnd9.Text);
            }
            if (textBoxAcctno10.Visible == true && !hashtableHeaders.ContainsKey("10"))
            {
                sw.WriteLine("Acctno10=" + textBoxAcctno10.Text);
                sw.WriteLine("Year10=" + textBoxYear10.Text);
                sw.WriteLine("Callfrom10=" + textBoxCallfrom10.Text);
                sw.WriteLine("PageStart10=" + textBoxPageStart10.Text);
                sw.WriteLine("PageEnd10=" + textBoxPageEnd10.Text);
            }

            sw.Close();
            File.Move(batchTempFile, batchConfigFile);

            this.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.Close();
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            this.Height = 480;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (textBoxAcctno2.Visible == false)
            {
                textBoxAcctno2.Visible = true;
                textBoxYear2.Visible = true;
                textBoxCallfrom2.Visible = true;
                textBoxPageStart2.Visible = true;
                labelTo2.Visible = true;
                textBoxPageEnd2.Visible = true;
            }
            else if (textBoxAcctno3.Visible == false)
            {
                textBoxAcctno3.Visible = true;
                textBoxYear3.Visible = true;
                textBoxCallfrom3.Visible = true;
                textBoxPageStart3.Visible = true;
                labelTo3.Visible = true;
                textBoxPageEnd3.Visible = true;
            }
            else if (textBoxAcctno4.Visible == false)
            {
                textBoxAcctno4.Visible = true;
                textBoxYear4.Visible = true;
                textBoxCallfrom4.Visible = true;
                textBoxPageStart4.Visible = true;
                labelTo4.Visible = true;
                textBoxPageEnd4.Visible = true;
            }
            else if (textBoxAcctno5.Visible == false)
            {
                textBoxAcctno5.Visible = true;
                textBoxYear5.Visible = true;
                textBoxCallfrom5.Visible = true;
                textBoxPageStart5.Visible = true;
                labelTo5.Visible = true;
                textBoxPageEnd5.Visible = true;
            }
            else if (textBoxAcctno6.Visible == false)
            {
                textBoxAcctno6.Visible = true;
                textBoxYear6.Visible = true;
                textBoxCallfrom6.Visible = true;
                textBoxPageStart6.Visible = true;
                labelTo6.Visible = true;
                textBoxPageEnd6.Visible = true;
            }
            else if (textBoxAcctno7.Visible == false)
            {
                textBoxAcctno7.Visible = true;
                textBoxYear7.Visible = true;
                textBoxCallfrom7.Visible = true;
                textBoxPageStart7.Visible = true;
                labelTo7.Visible = true;
                textBoxPageEnd7.Visible = true;
            }
            else if (textBoxAcctno8.Visible == false)
            {
                textBoxAcctno8.Visible = true;
                textBoxYear8.Visible = true;
                textBoxCallfrom8.Visible = true;
                textBoxPageStart8.Visible = true;
                labelTo8.Visible = true;
                textBoxPageEnd8.Visible = true;
            }
            else if (textBoxAcctno9.Visible == false)
            {
                textBoxAcctno9.Visible = true;
                textBoxYear9.Visible = true;
                textBoxCallfrom9.Visible = true;
                textBoxPageStart9.Visible = true;
                labelTo9.Visible = true;
                textBoxPageEnd9.Visible = true;
            }
            else if (textBoxAcctno10.Visible == false)
            {
                textBoxAcctno10.Visible = true;
                textBoxYear10.Visible = true;
                textBoxCallfrom10.Visible = true;
                textBoxPageStart10.Visible = true;
                labelTo10.Visible = true;
                textBoxPageEnd10.Visible = true;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BatchProperties_Load(object sender, EventArgs e)
        {
            string batchConfigFile = BatchPath + "\\batch.config", line, headerNumber, fieldName, fieldValue;
            TextBox textBox;

            if (File.Exists(batchConfigFile))
            {
                using (StreamReader sr = new StreamReader(batchConfigFile))
                {
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("Acctno") || line.StartsWith("Year") || line.StartsWith("Callfrom") || line.StartsWith("PageStart") || line.StartsWith("PageEnd"))
                        {
                            fieldName = line.Split('=')[0];
                            fieldValue = line.Split('=')[1];
                            headerNumber = fieldName.Substring(fieldName.Length - 1, 1);
                            headerNumber = headerNumber == "0" ? "10" : headerNumber;
                            textBox = this.Controls.Find("textBox" + fieldName, true)[0] as TextBox;
                            textBox.Visible = true;
                            textBox.Text = fieldValue;
                        }
                        else if (line.StartsWith("BatchType"))
                        {
                            fieldValue = line.Split('=')[1];

                            if (fieldValue == "BankStatement")
                                radioButtonBankStatement.Checked = true;
                            else
                                radioButtonPhoneBill.Checked = true;
                        }
                    }
                }
            }
        }
    }
}
