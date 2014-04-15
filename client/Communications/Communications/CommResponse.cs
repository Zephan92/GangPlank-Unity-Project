namespace Gangplank.Communications {
    public class CommResponse {
        private string tostr;
        public CommResponse(bool success, string type, string message) {
            this.success = success;
				this.type = type;
            this.message = message;
            tostr = "{"+this.success+", "+this.type+", "+this.message+"}";
        }
        public bool success { get; private set; }
        public string message { get; private set; }
		  public string type{ get; private set; }

        public override string ToString() {
            return tostr;
        }
    }
}
