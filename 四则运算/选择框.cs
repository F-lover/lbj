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
