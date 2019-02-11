using System.Reflection;
using System.Windows.Forms;

namespace YetAnotherRelogger.Helpers
{
    public static class DataGridViewExtensions
    {
        public static void DoubleBuffered(this DataGridView dgv, bool setting)
        {
            var dgvType = dgv.GetType();
            var pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
            pi?.SetValue(dgv, setting, null);
        }
    }
}
