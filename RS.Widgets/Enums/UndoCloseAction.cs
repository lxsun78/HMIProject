using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RS.Widgets.Enums
{
    /// <summary>
    /// Specifies the type of change applied to TextContainer content.
    /// </summary>
    public enum UndoCloseAction
    {
        /// <summary>
        /// Keep the open undo unit.
        /// </summary>
        Commit,

        /// <summary>
        /// Rollback the undo undo.  This calls unit.Do to undo the changes.
        /// </summary>
        Rollback,

        /// <summary>
        /// Throw away the undo unit without calling unit.Do.
        /// BE CAREFUL!  If the unit contains any changes that modified the
        /// state of the underlying content, the undo stack may be corrupt.
        /// </summary>
        Discard,
    }
}
