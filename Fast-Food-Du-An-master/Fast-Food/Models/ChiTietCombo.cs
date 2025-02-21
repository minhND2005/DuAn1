using System;
using System.Collections.Generic;

namespace Fast_Food.Models;

public partial class ChiTietCombo
{
    public int MaCombo { get; set; }

    public int MaMon { get; set; }

    public virtual MonAn MaMonNavigation { get; set; } = null!;
}
