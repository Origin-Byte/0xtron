# origin-byte-tron

![img1](/imgs/img1.png "on-chain tron game")
![img2](/imgs/img2.png "on-chain tron game")
![img3](/imgs/img3.png "on-chain tron game")

.io style multiplayer survival bike racing game. Avoid collision with your and the enemies' trails

## Play here
https://onchaintron.z6.web.core.windows.net/

## Gameplay video
https://youtu.be/04DjrAhQPLU

## Made with Origin Byte Sui Unity SDK
game clients use the Sui blockchain directly as a synchronization backend

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
	- note: you may be rate limited, check the official Sui faucet discord channel if it's working https://discord.com/channels/916379725201563759/971488439931392130
- hit play
- players spawn at random positions in the arena
- forward movement is automatic, turn with A/D or Left/Right arrow buttons
- survive as long as you can by avoiding trail colliders
	
## Current issues
- Leaderboard is disabled right now! Writing to the scoreboard stops the event stream from the blockchain for several seconds. Requires further analysis of the move code and Sui transaction locks.
- video: https://youtu.be/FTq3Fjr_dBc

## if you encounter issues, please report it with your Sui Address and browser log!