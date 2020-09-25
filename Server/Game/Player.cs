namespace Server {
    public class Player {
        public ushort ID { get; set; }
        
        public string Session { get; set; }
        
        public string Name { get; set; }
        public ushort Avatar { get; set; }
        
        // Position
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float PositionZ { get; set; }
        
        // Animators params
        public bool AnimatorIsWalking { get; set; }
        public bool AnimatorIsRunning { get; set; }
        public bool AnimatorIsBackWalking { get; set; }
        public bool AnimatorIsRightWalking { get; set; }
        public bool AnimatorIsLeftWalking { get; set; }
        public bool AnimatorIsDirectRightWalking { get; set; }
        public bool AnimatorIsDirectLeftWalking { get; set; }
        public bool AnimatorIsJumpRun { get; set; }
        
        // Rotation
        public float RotationX { get; set; }
        public float RotationY { get; set; }
        public float RotationZ { get; set; }
        public float RotationW { get; set; }
        
        public Player(ushort playerId) {
            
            ID = playerId;
            Session = "";
            Name = "";
            Avatar = AvatarType.Neru;

            // Position
            PositionX = 0.0f;
            PositionY = 0.0f;
            PositionZ = 0.0f;
            
            // Animator params
            AnimatorIsWalking = false;
            AnimatorIsRunning = false;
            AnimatorIsBackWalking = false;
            AnimatorIsRightWalking = false;
            AnimatorIsLeftWalking = false;
            AnimatorIsDirectRightWalking = false;
            AnimatorIsDirectLeftWalking = false;
            AnimatorIsJumpRun = false;
            
            // Rotation
            RotationX = 0.0f;
            RotationY = 0.0f;
            RotationZ = 0.0f;
            RotationW = 0.0f;
        }
    }
}