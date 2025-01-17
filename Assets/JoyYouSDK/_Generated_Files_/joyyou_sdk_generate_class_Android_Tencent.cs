namespace Assets.SDK
{
#if QA_TEST
    [InitAndroidTencentAttribute(1105309683, "xa0seqAScOhSsgrm", "wxfdab5af74990787a", "1450007239", "02a8d5ed226237996eb3f448dfac0b1c", "XGamePoint", null, false, true, true, false, true)]
#elif RECHARGE_TEST
    [InitAndroidTencentAttribute(1105309683, "xa0seqAScOhSsgrm", "wxfdab5af74990787a", "1450007239", "02a8d5ed226237996eb3f448dfac0b1c", "XGamePoint", null, false, true, true, false, true)]
#else
    [InitAndroidTencentAttribute(1105309683, "xa0seqAScOhSsgrm", "wxfdab5af74990787a", "1450007239", "02a8d5ed226237996eb3f448dfac0b1c", "XGamePoint", null, false, true, true, false, false)]
#endif
    public partial class JoyYouSDK {}
}
