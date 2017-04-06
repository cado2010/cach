using System.Windows.Forms;

namespace cach
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();
        }
    }
}
