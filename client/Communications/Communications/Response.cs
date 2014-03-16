using System;

namespace Gangplank.Communications {
    public class Response {
        private string tostr;
        public Response(bool success, string message) {
            this.success = success;
            this.message = message;
            tostr = "{"+this.success+", "+this.message+"}";
        }
        public bool success { get; private set; }
        public string message { get; private set; }

        public override string ToString() {
            return tostr;
        }
    }
}
