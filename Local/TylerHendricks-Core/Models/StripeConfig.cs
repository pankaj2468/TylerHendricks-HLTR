namespace TylerHendricks_Core.Models
{
    public class StripeConfig
    {
        public string StripeKey { get; set; }
        public string StripeJSKey { get; set; }
        public string SuccessUrl { get; set; }
        public string SuccessUrlChat { get; set; }
        public string CancelUrl { get; set; }
        public string ErectileDysfunctionPaymentCancelUrl { get; set; }
        public string MedicalRefillPaymentCancelUrl { get; set; }
        public string HairLossPaymentCancelUrl { get; set; }
    }
}
