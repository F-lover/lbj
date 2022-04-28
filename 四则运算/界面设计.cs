using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyExpression;
using System.IO;

namespace 四则运算_生成
{
    public partial class FrmQuizGen : Form
    {
        //private ExprBuilder eBuilder;
        private List<Expr> questions = new List<Expr>();//存储生成题目列表
        private class QuizConfig
        {
            public ExprBuilder eBuilder;
            public int qcount;
            //public bool CplxFlag;
            public int addcount, subcount, mulcount, divcount;
            public int Count
            {
                get
                {
                    if (eBuilder.allowCplxExpr)
                        return qcount;
                    else
                        return addcount + subcount + mulcount + divcount;
                }
            }
            public QuizConfig(int addcount, int subcount, int mulcount, int divcount)
            {
                eBuilder = new ExprBuilder();
                this.addcount = addcount;
                this.subcount = subcount;
                this.mulcount = mulcount;
                this.divcount = divcount;
            }
        }
        QuizConfig qConfig;
        public FrmQuizGen()
        {
            qConfig = new QuizConfig(0, 0, 0, 0);
            InitializeComponent();
        }

        private void stpQSub_ValueChanged(object sender, EventArgs e)
        {
            qConfig.subcount = decimal.ToInt32((sender as NumericUpDown).Value);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            questions.Clear();
            BindingSource bs = new BindingSource();
            bs.DataSource = questions;
            lBoxQuizDisp.DataSource = bs;
            lBoxQuizDisp.DisplayMember = "";

        }

        private void FrmQuizGen_Activated(object sender, EventArgs e)
        {
            UpdateCplxSettings();
            UpdateStepper();
            stpMaxPrecision.Enabled = cBoxAllowFrac.Checked;
            BindingSource bs =  new BindingSource();
            bs.DataSource = questions;
            lBoxQuizDisp.DataSource = bs;
            lBoxQuizDisp.DisplayMember = "";
        }

        private void cBoxQTypeAdd_CheckedChanged(object sender, EventArgs e)
        {
            stpQAdd.Enabled = cBoxQTypeAdd.Checked && cBoxQTypeAdd.Enabled;
            if ((sender as CheckBox).Checked)
            {
                qConfig.eBuilder.Allow(ExprOprt.ADD);
            }
            else
            {
                qConfig.eBuilder.Disallow(ExprOprt.ADD);
            }
            stpQAdd_ValueChanged(stpQAdd, new EventArgs());
        }

        private void cBoxQTypeSub_CheckedChanged(object sender, EventArgs e)
        {
            stpQSub.Enabled = cBoxQTypeSub.Checked && cBoxQTypeSub.Enabled;
            if ((sender as CheckBox).Checked)
            {
                qConfig.eBuilder.Allow(ExprOprt.SUB);
            }
            else
            {
                qConfig.eBuilder.Disallow(ExprOprt.SUB);
            }
            stpQSub_ValueChanged(stpQSub, new EventArgs());
        }

        private void cBoxQTypeMul_CheckedChanged(object sender, EventArgs e)
        {
            stpQMul.Enabled = cBoxQTypeMul.Checked && cBoxQTypeMul.Enabled;
            if ((sender as CheckBox).Checked)
            {
                qConfig.eBuilder.Allow(ExprOprt.MUL);
            }
            else
            {
                qConfig.eBuilder.Disallow(ExprOprt.MUL);
            }
            stpQMul_ValueChanged(stpQMul, new EventArgs());
        }

        private void cBoxQTypeDiv_CheckedChanged(object sender, EventArgs e)
        {
            stpQDiv.Enabled = cBoxQTypeDiv.Checked && cBoxQTypeDiv.Enabled;
            if ((sender as CheckBox).Checked)
            {
                qConfig.eBuilder.Allow(ExprOprt.DIV);
            }
            else
            {
                qConfig.eBuilder.Disallow(ExprOprt.DIV);
            }
            stpQDiv_ValueChanged(stpQDiv, new EventArgs());
        }

        private void stpMaxOptr_ValueChanged(object sender, EventArgs e)
        {
            qConfig.eBuilder.maxOpnd = decimal.ToInt32(stpMaxOptr.Value);
        }

        private void cBoxAllowCplx_CheckedChanged(object sender, EventArgs e)
        {
            UpdateCplxSettings();
            UpdateStepper();
            qConfig.eBuilder.allowCplxExpr = (sender as CheckBox).Checked;
            if(qConfig.eBuilder.allowCplxExpr)
            {
                cBoxCQuizAdd_CheckedChanged(cBoxCQuizAdd, new EventArgs());
                cBoxCQuizSub_CheckedChanged(cBoxCQuizSub, new EventArgs());
                cBoxCQuizMul_CheckedChanged(cBoxCQuizMul, new EventArgs());
                cBoxCQuizDiv_CheckedChanged(cBoxCQuizDiv, new EventArgs());
            }
            else
            {
                cBoxQTypeAdd_CheckedChanged(cBoxQTypeAdd, new EventArgs());
                cBoxQTypeSub_CheckedChanged(cBoxQTypeSub, new EventArgs());
                cBoxQTypeMul_CheckedChanged(cBoxQTypeMul, new EventArgs());
                cBoxQTypeDiv_CheckedChanged(cBoxQTypeDiv, new EventArgs());
                qConfig.addcount = decimal.ToInt32(stpQAdd.Value);
                qConfig.subcount = decimal.ToInt32(stpQSub.Value);
                qConfig.mulcount = decimal.ToInt32(stpQMul.Value);
                qConfig.divcount = decimal.ToInt32(stpQDiv.Value);
            }
        }

        private void UpdateCplxSettings()
        {
            cBoxQTypeAdd.Enabled = !cBoxAllowCplx.Checked;
            cBoxQTypeSub.Enabled = !cBoxAllowCplx.Checked;
            cBoxQTypeMul.Enabled = !cBoxAllowCplx.Checked;
            cBoxQTypeDiv.Enabled = !cBoxAllowCplx.Checked;

            labPromptCQuizNum.Enabled = cBoxAllowCplx.Checked;
            labPromptMaxOptr.Enabled = cBoxAllowCplx.Checked;
            labPromptAllowType.Enabled = cBoxAllowCplx.Checked;
            cBoxCQuizAdd.Enabled = cBoxAllowCplx.Checked;
            cBoxCQuizSub.Enabled = cBoxAllowCplx.Checked;
            cBoxCQuizMul.Enabled = cBoxAllowCplx.Checked;
            cBoxCQuizDiv.Enabled = cBoxAllowCplx.Checked;
            cBoxCQuizAllowBrack.Enabled = cBoxAllowCplx.Checked;
            stpCQuizNum.Enabled = cBoxAllowCplx.Checked;
            stpMaxOptr.Enabled = cBoxAllowCplx.Checked;
        }

        private void UpdateStepper()
        {
            stpQAdd.Enabled = cBoxQTypeAdd.Checked && cBoxQTypeAdd.Enabled;
            stpQSub.Enabled = cBoxQTypeSub.Checked && cBoxQTypeSub.Enabled;
            stpQMul.Enabled = cBoxQTypeMul.Checked && cBoxQTypeMul.Enabled;
            stpQDiv.Enabled = cBoxQTypeDiv.Checked && cBoxQTypeDiv.Enabled;
        }

        private void cBoxAllowFrac_CheckedChanged(object sender, EventArgs e)
        {
            stpMaxPrecision.Enabled = cBoxAllowFrac.Checked;
            qConfig.eBuilder.allowDec = (sender as CheckBox).Checked;
        }

        private void stpCQuizNum_ValueChanged(object sender, EventArgs e)
        {
            qConfig.qcount = decimal.ToInt32((sender as NumericUpDown).Value);
        }

        private void cBoxCQuizAdd_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                qConfig.eBuilder.Allow(ExprOprt.ADD);
            }
            else
            {
                qConfig.eBuilder.Disallow(ExprOprt.ADD);
            }
        }

        private void cBoxCQuizSub_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                qConfig.eBuilder.Allow(ExprOprt.SUB);
            }
            else
            {
                qConfig.eBuilder.Disallow(ExprOprt.SUB);
            }
        }

        private void cBoxCQuizMul_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                qConfig.eBuilder.Allow(ExprOprt.MUL);
            }
            else
            {
                qConfig.eBuilder.Disallow(ExprOprt.MUL);
            }
        }

        private void cBoxCQuizDiv_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as CheckBox).Checked)
            {
                qConfig.eBuilder.Allow(ExprOprt.DIV);
            }
            else
            {
                qConfig.eBuilder.Disallow(ExprOprt.DIV);
            }
        }

        private void cBoxCQuizAllowBrack_CheckedChanged(object sender, EventArgs e)
        {
            qConfig.eBuilder.allowBrack = (sender as CheckBox).Checked;
        }

        private void stpMaxPrecision_ValueChanged(object sender, EventArgs e)
        {
            qConfig.eBuilder.maxPrecision = decimal.ToInt32((sender as NumericUpDown).Value);
        }

        private void stpQAdd_ValueChanged(object sender, EventArgs e)
        {
            qConfig.addcount = decimal.ToInt32((sender as NumericUpDown).Value);
        }

        private void stpQMul_ValueChanged(object sender, EventArgs e)
        {
            qConfig.mulcount = decimal.ToInt32((sender as NumericUpDown).Value);
        }

        private void stpQDiv_ValueChanged(object sender, EventArgs e)
        {
            qConfig.divcount = decimal.ToInt32((sender as NumericUpDown).Value);
        }

        private void lBoxQuizDisp_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            try
            {
                GenerateQuestions(r);
            }
            catch (Exception)
            {
                MessageBox.Show("未选定合适的运算符！");
            }
            BindingSource bs = new BindingSource();
            bs.DataSource = questions;
            lBoxQuizDisp.DataSource = bs;
            lBoxQuizDisp.DisplayMember = "";
            return;
        }

        private void GenerateQuestions(Random r)
        {
            if(qConfig.eBuilder.allowOprt == (byte)ExprOprt.NIL)
            {
                throw new NoOprtFoundException();
            }
            if(qConfig.eBuilder.allowCplxExpr)
            {
                for (var i = 0; i < qConfig.Count; i++)
                {
                    try
                    {
                        Expr egen = qConfig.eBuilder.Generate(r);
                        if (egen == null)
                        {
                            i--;
                            continue;
                        }
                        questions.Add(egen);
                    }
                    catch (Exception)
                    {
                        i--;
                        continue;
                    }
                    //questions.Add(egen);
                }

            }
            else
            {
                int i;
                for (i = 0; i < qConfig.addcount;i++)
                {
                    try
                    {
                        Expr egen = qConfig.eBuilder.BinaryGenerate(ExprOprt.ADD,r);
                        if(egen == null)
                        {
                            i--;
                            continue;
                        }
                        questions.Add(egen);
                    }
                    catch(Exception e)
                    {
                        i--;
                        continue;
                    }
                }
                for (i = 0; i < qConfig.subcount; i++)
                {
                    try
                    {
                        Expr egen = qConfig.eBuilder.BinaryGenerate(ExprOprt.SUB,r);
                        if (egen == null)
                        {
                            i--;
                            continue;
                        }
                        questions.Add(egen);
                    }
                    catch (Exception e)
                    {
                        i--;
                        continue;
                    }
                }
                for (i = 0; i < qConfig.mulcount; i++)
                {
                    try
                    {
                        Expr egen = qConfig.eBuilder.BinaryGenerate(ExprOprt.MUL,r);
                        if (egen == null)
                        {
                            i--;
                            continue;
                        }
                        questions.Add(egen);
                    }
                    catch (Exception e)
                    {
                        i--;
                        continue;
                    }
                }
                for (i = 0; i < qConfig.divcount; i++)
                {
                    try
                    {
                        Expr egen = qConfig.eBuilder.BinaryGenerate(ExprOprt.DIV,r);
                        if (egen == null)
                        {
                            i--;
                            continue;
                        }
                        questions.Add(egen);
                    }
                    catch (Exception e)
                    {
                        i--;
                        continue;
                    }
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StreamWriter wstream;
            sfdQuestionstxt.Filter = "文本文件 (*.txt)|*.txt";
            sfdQuestionstxt.RestoreDirectory = true;
            if(questions.Count > 0)
            {
                if (sfdQuestionstxt.ShowDialog() == DialogResult.OK)
                {
                    wstream = new StreamWriter(sfdQuestionstxt.FileName);
                    foreach (var i in questions)
                    {
                        wstream.WriteLine(string.Format("{0}=",i));
                    }
                    wstream.Flush();
                    wstream.Close();
                    MessageBox.Show("保存成功");
                }
                else
                {
                    MessageBox.Show("保存取消");
                }
            }
            else
            {
                MessageBox.Show("没有题目");
            }
        }

        private void sfdQuestionstxt_FileOk(object sender, CancelEventArgs e)
        {

        }
    }
}
