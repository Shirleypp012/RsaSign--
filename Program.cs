using System;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace RsaSignDemo
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        private readonly TextBox txtInput = new TextBox();
        private readonly Button btnHash = new Button();
        private readonly TextBox txtHashResult = new TextBox();
        private readonly Button btnSign = new Button();
        private readonly TextBox txtSignResult = new TextBox();
        private readonly Button btnVerify = new Button();
        private readonly TextBox txtVerifyResult = new TextBox();

        private RSAParameters privateKey;
        private RSAParameters publicKey;

        public MainForm()
        {
            Text = "RSA数字签名演示";
            Width = 600;
            Height = 370;
            StartPosition = FormStartPosition.CenterScreen;

            BuildLayout();
            InitRsaKey();
        }

        private void BuildLayout()
        {
            var lblInput = new Label
            {
                Text = "待签名字符串：",
                Left = 12,
                Top = 16,
                Width = 90
            };

            txtInput.Left = 110;
            txtInput.Top = 12;
            txtInput.Width = 455;

            btnHash.Text = "1. hash结果";
            btnHash.Left = 12;
            btnHash.Top = 50;
            btnHash.Width = 90;
            btnHash.Height = 35;
            btnHash.Click += BtnHash_Click;

            txtHashResult.Left = 110;
            txtHashResult.Top = 50;
            txtHashResult.Width = 455;
            txtHashResult.Height = 70;
            txtHashResult.Multiline = true;
            txtHashResult.ScrollBars = ScrollBars.Vertical;

            btnSign.Text = "2. 签名结果";
            btnSign.Left = 12;
            btnSign.Top = 132;
            btnSign.Width = 90;
            btnSign.Height = 35;
            btnSign.Click += BtnSign_Click;

            txtSignResult.Left = 110;
            txtSignResult.Top = 132;
            txtSignResult.Width = 455;
            txtSignResult.Height = 70;
            txtSignResult.Multiline = true;
            txtSignResult.ScrollBars = ScrollBars.Vertical;

            btnVerify.Text = "3. 验证结果";
            btnVerify.Left = 12;
            btnVerify.Top = 214;
            btnVerify.Width = 90;
            btnVerify.Height = 35;
            btnVerify.Click += BtnVerify_Click;

            txtVerifyResult.Left = 110;
            txtVerifyResult.Top = 214;
            txtVerifyResult.Width = 455;
            txtVerifyResult.Height = 100;
            txtVerifyResult.Multiline = true;
            txtVerifyResult.ScrollBars = ScrollBars.Vertical;

            Controls.Add(lblInput);
            Controls.Add(txtInput);
            Controls.Add(btnHash);
            Controls.Add(txtHashResult);
            Controls.Add(btnSign);
            Controls.Add(txtSignResult);
            Controls.Add(btnVerify);
            Controls.Add(txtVerifyResult);
        }

        private void InitRsaKey()
        {
            using (var rsa = new RSACryptoServiceProvider(2048))
            {
                privateKey = rsa.ExportParameters(true);
                publicKey = rsa.ExportParameters(false);
            }
        }

        private void BtnHash_Click(object sender, EventArgs e)
        {
            var plain = txtInput.Text.Trim();
            if (string.IsNullOrEmpty(plain))
            {
                MessageBox.Show("请输入待签名内容。");
                return;
            }

            var hash = Sha256(plain);
            txtHashResult.Text = Convert.ToBase64String(hash);
        }

        private void BtnSign_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHashResult.Text))
            {
                MessageBox.Show("请先生成 hash 结果。");
                return;
            }

            byte[] hash;
            try
            {
                hash = Convert.FromBase64String(txtHashResult.Text.Trim());
            }
            catch
            {
                MessageBox.Show("hash结果格式错误（应为Base64）。");
                return;
            }

            var sign = RsaPrivateEncrypt(hash, privateKey);
            txtSignResult.Text = Convert.ToBase64String(sign);
        }

        private void BtnVerify_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHashResult.Text) || string.IsNullOrWhiteSpace(txtSignResult.Text))
            {
                MessageBox.Show("请先生成 hash 结果和签名结果。");
                return;
            }

            byte[] hash2;
            byte[] signBytes;
            try
            {
                hash2 = Convert.FromBase64String(txtHashResult.Text.Trim());
                signBytes = Convert.FromBase64String(txtSignResult.Text.Trim());
            }
            catch
            {
                MessageBox.Show("输入格式错误（应为Base64）。");
                return;
            }

            var result1 = RsaPublicDecrypt(signBytes, publicKey, hash2.Length);
            var pass = result1.SequenceEqual(hash2);

            txtVerifyResult.Text =
                "结果1(公钥解密): " + Convert.ToBase64String(result1) + Environment.NewLine +
                "结果2(hash): " + Convert.ToBase64String(hash2) + Environment.NewLine +
                (pass ? "验证通过" : "验证失败");
        }

        private static byte[] Sha256(string text)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(text));
            }
        }

        // 教学演示：将 hash 视作整数，执行 m^d mod n
        private static byte[] RsaPrivateEncrypt(byte[] data, RSAParameters key)
        {
            var m = FromBigEndianUnsigned(data);
            var d = FromBigEndianUnsigned(key.D);
            var n = FromBigEndianUnsigned(key.Modulus);
            var c = BigInteger.ModPow(m, d, n);
            return ToBigEndianUnsigned(c, key.Modulus.Length);
        }

        // 教学演示：执行 c^e mod n，得到“验证结果1”
        private static byte[] RsaPublicDecrypt(byte[] cipher, RSAParameters key, int expectedLen)
        {
            var c = FromBigEndianUnsigned(cipher);
            var e = FromBigEndianUnsigned(key.Exponent);
            var n = FromBigEndianUnsigned(key.Modulus);
            var m = BigInteger.ModPow(c, e, n);
            return ToBigEndianUnsigned(m, expectedLen);
        }

        private static BigInteger FromBigEndianUnsigned(byte[] bytes)
        {
            var little = new byte[bytes.Length + 1];
            for (var i = 0; i < bytes.Length; i++)
            {
                little[i] = bytes[bytes.Length - 1 - i];
            }
            little[bytes.Length] = 0x00;
            return new BigInteger(little);
        }

        private static byte[] ToBigEndianUnsigned(BigInteger value, int fixedLength)
        {
            var little = value.ToByteArray();
            var len = little.Length;
            while (len > 1 && little[len - 1] == 0x00)
            {
                len--;
            }

            var big = new byte[len];
            for (var i = 0; i < len; i++)
            {
                big[i] = little[len - 1 - i];
            }

            if (big.Length == fixedLength)
            {
                return big;
            }

            if (big.Length > fixedLength)
            {
                return big.Skip(big.Length - fixedLength).ToArray();
            }

            var padded = new byte[fixedLength];
            Buffer.BlockCopy(big, 0, padded, fixedLength - big.Length, big.Length);
            return padded;
        }
    }
}
