namespace ETLProcess.General.Interfaces.Profile_Interfaces
{
    interface IA_OutputProfile
    {
        public bool Check_Output(DelRet<bool, string[]> outputs);
        public void Export();
    }
}
