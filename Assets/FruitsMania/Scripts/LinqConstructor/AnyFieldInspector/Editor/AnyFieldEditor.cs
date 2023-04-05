using System.Collections;

namespace dotmob.Scripts.LinqConstructor.AnyFieldInspector.Editor
{
    public class AnyFieldEditor : UnityEditor.Editor
    {
        public void ShowList(IList list)
        {
            GuiList.Show(list);
        }
    }
}