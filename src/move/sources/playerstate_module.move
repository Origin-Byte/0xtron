module origin_byte_game::playerstate_module {
    use sui::object::{Self, UID};
    use sui::transfer;
    use sui::tx_context::{Self, TxContext};
    
    const SIGNED_OFFSET : u64 = 10000000;

    struct Vector2 has copy, store, drop {
        x: u64,
        y: u64
    }

    struct PlayerState has key {
        id: UID,
        position: Vector2,
        velocity: Vector2,
        // used to reject out of order updates
        sequenceNum: u64,
        isExploded: bool
    }

    struct PlayerStateUpdatedEvent has copy, drop {
        position: Vector2,
        velocity: Vector2,
        sequenceNum: u64,
        isExploded : bool
    }

    public entry fun create_playerstate_for_sender(ctx: &mut TxContext) {
        let state = PlayerState {
            id: object::new(ctx),
            position: Vector2 {
                x: SIGNED_OFFSET,
                y: SIGNED_OFFSET
            },
            velocity: Vector2 {
                x: SIGNED_OFFSET,
                y: SIGNED_OFFSET
            },
            sequenceNum: 0,
            isExploded: false
        };
        transfer::transfer(state, tx_context::sender(ctx));
    }

    public entry fun reset(self: &mut PlayerState) {
        use sui::event;

        self.position.x = SIGNED_OFFSET;
        self.position.y = SIGNED_OFFSET;
        self.velocity.x = SIGNED_OFFSET;
        self.velocity.y = SIGNED_OFFSET;
        self.sequenceNum = 0;
        self.isExploded = false;

        event::emit(PlayerStateUpdatedEvent { position: self.position, velocity: self.velocity, sequenceNum: self.sequenceNum, isExploded: self.isExploded })
    }

    public entry fun do_update(self: &mut PlayerState, posX: u64, posY: u64, velX: u64, velY: u64, sequenceNum: u64, isExploded: bool) {
        use sui::event;
        
        if (sequenceNum > self.sequenceNum) {     
            self.position.x = posX;
            self.position.y = posY;
            self.velocity.x = velX;
            self.velocity.y = velY;
            self.sequenceNum = sequenceNum;
            self.isExploded = isExploded;

            event::emit(PlayerStateUpdatedEvent { position: self.position, velocity: self.velocity, sequenceNum: self.sequenceNum, isExploded: self.isExploded })
        };
    }
}