module origin_byte_game::playerstate_module {
    use sui::object::{Self, UID};
    use sui::transfer;
    use sui::tx_context::{Self, TxContext};
    use std::vector;

    const SIGNED_OFFSET : u64 = 10000000;

    struct Vector2 has copy, store, drop {
        x: u64,
        y: u64
    }

    struct ScoreBoardElement has key, store {
        id: UID,
        player: address,
        score: u64
    }

    struct ScoreBoard has key {
        id: UID,
       // admin: address,
        scores: vector<ScoreBoardElement>
    }

    struct PlayerState has key {
        id: UID,
        position: Vector2,
        velocity: Vector2,
        // used to reject out of order updates
        sequenceNum: u64,
        isExploded: bool,
        createdTimestamp: u64,
        lastUpdateTimestamp: u64
    }

    struct PlayerStateUpdatedEvent has copy, drop {
        position: Vector2,
        velocity: Vector2,
        sequenceNum: u64,
        isExploded : bool,
        createdTimestamp: u64,
        lastUpdateTimestamp: u64
    }

    fun init(ctx: &mut TxContext) {
        let scoreboard = ScoreBoard {
            id: object::new(ctx),
            scores: vector::empty<ScoreBoardElement>()
        };

        transfer::share_object(scoreboard);
    }

    public entry fun create_playerstate_for_sender(timestamp: u64, ctx: &mut TxContext) {
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
            isExploded: false,
            createdTimestamp: timestamp,
            lastUpdateTimestamp: timestamp
        };
        transfer::transfer(state, tx_context::sender(ctx));
    }

    public entry fun reset(self: &mut PlayerState, posX: u64, posY: u64, timestamp: u64) {
        use sui::event;
      
        self.position.x = posX;
        self.position.y = posY;
        self.velocity.x = SIGNED_OFFSET;
        self.velocity.y = SIGNED_OFFSET;
        self.sequenceNum = 0;
        self.isExploded = false;
        self.createdTimestamp = timestamp;
        self.lastUpdateTimestamp = timestamp;
        event::emit(PlayerStateUpdatedEvent { position: self.position, velocity: self.velocity, sequenceNum: self.sequenceNum, isExploded: self.isExploded, createdTimestamp: self.createdTimestamp, lastUpdateTimestamp: self.lastUpdateTimestamp })
    }

    public entry fun do_update(self: &mut PlayerState, posX: u64, posY: u64, velX: u64, velY: u64, sequenceNum: u64, isExploded: bool, timestamp: u64) {
        use sui::event;
        
        if (sequenceNum > self.sequenceNum) {     
            self.position.x = posX;
            self.position.y = posY;
            self.velocity.x = velX;
            self.velocity.y = velY;
            self.sequenceNum = sequenceNum;
            self.isExploded = isExploded;
            self.lastUpdateTimestamp = timestamp;
            
            event::emit(PlayerStateUpdatedEvent { position: self.position, velocity: self.velocity, sequenceNum: self.sequenceNum, isExploded: self.isExploded, createdTimestamp: self.createdTimestamp, lastUpdateTimestamp: self.lastUpdateTimestamp })
        };
    }

    public entry fun add_to_scoreboard(
        player_state: &PlayerState,
        scoreboard: &mut ScoreBoard,
        ctx: &mut TxContext
    ) {

        let element = ScoreBoardElement {
            id: object::new(ctx),
            player: sui::tx_context::sender(ctx),
            score: (player_state.lastUpdateTimestamp - player_state.createdTimestamp) / 1000
        };
        vector::push_back(&mut scoreboard.scores, element);
    }
}