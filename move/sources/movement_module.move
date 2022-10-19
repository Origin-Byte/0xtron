module origin_byte_game::movement_module {
    use sui::object::{Self, UID};
    use sui::transfer;
    use sui::tx_context::{Self, TxContext};

    struct Position has key, store {
        id: UID,
        x: u64,
        y: u64
    }

    struct PositionUpdatedEvent has copy, drop {
        x: u64,
        y: u64,
        owner: address
    }

    public entry fun create_position_for_sender(ctx: &mut TxContext) {
        let pos = Position {
            id: object::new(ctx),
            x: 100000,
            y: 100000
        };
        transfer::transfer(pos, tx_context::sender(ctx));
    }

    public entry fun do_move(self: &mut Position, x: u8, y: u8, ctx: &mut TxContext) {
        use sui::event;
        
        if (x > 1){
            self.x = self.x + 1;
        };
        if (x < 1 && self.x > 0){
            self.x = self.x - 1;
        };
        if (y > 1){
            self.y = self.y + 1;
        };
        if (y < 1 && self.y > 0){
            self.y = self.y - 1;
        };

        event::emit(PositionUpdatedEvent { x: self.x, y: self.y, owner: tx_context::sender(ctx)})
    }
}