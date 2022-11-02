# origin-byte-tron
=========

.io style multiplayer survival bike racing game. Avoid collision with your and the enemies' trails

## Play here
https://onchaintron.z6.web.core.windows.net/

## Gameplay video
https://youtu.be/04DjrAhQPLU

## Made with Origin Byte Sui Unity SDK
game clients use the Sui blockchain directly as a synchronization backend
	- move structs:
	```
		struct ScoreBoardElement has key, store {
			id: UID,
			player: address,
			score: u64
		}

		struct ScoreBoard has key {
			id: UID,
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
	```
- on publishing the module, a shared Scoreboard object is created, that only players with playerstate can modify
- every player owns a playerstate move object, a new one is minted for every new game sessions (when a player hits Play)
- game clients that have a valid playerstate initialized update their state constantly, using ImmediateReturn type move call transactions
- game clients listen to events emitted from the origin_byte_game::playerstate_module
	- note: Sui has not published a streaming (websocket based) rpc endpoint yet, so it's polling based using sui_getEventsByModule rpc method
	- enemy players are spawned, updated, destroyed based on these events
	- trail colliders are updated based on these events

## Gameplay
- player can create / import wallet
- request airdrop if needed
	- note: you may be rate limited!
- hit play
- movement is automatic, turn with A/D or Left/Right arrow buttons
- survive as long as you can by avoiding trail colliders	
	
	
## Current issues
- scoreboard is disabled. Writing to the scoreboard stops the event stream from the blockchain for several seconds. Requires further analysis.