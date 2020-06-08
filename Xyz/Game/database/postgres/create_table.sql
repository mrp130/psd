CREATE TABLE "room"
(
    id          UUID PRIMARY KEY,
    max_player  INT NOT NULL,
    created_at  TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMPTZ,
    deleted_at  TIMESTAMPTZ
);

CREATE TABLE "user"
(
    id         UUID PRIMARY KEY,
    name       VARCHAR(100) NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ
);

CREATE TABLE "exp"
(
    id         UUID PRIMARY KEY,
    user_id    UUID NOT NULL,
    exp        INT  NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (user_id) REFERENCES "user" (id)
);

CREATE TABLE "room_player"
(
    user_id    UUID NOT NULL,
    room_id    UUID NOT NULL,
    FOREIGN KEY (user_id) REFERENCES "user" (id),
    FOREIGN KEY (room_id) REFERENCES "room" (id),
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ
);

CREATE TABLE "game"
(
    id         UUID PRIMARY KEY,
    room_id    UUID NOT NULL,
    game_type   VARCHAR(100),
    game_config JSONB,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    deleted_at TIMESTAMPTZ,
    FOREIGN KEY (room_id) REFERENCES "room" (id)
);

CREATE TABLE "game_move"
(
    id         UUID PRIMARY KEY,
    game_id    UUID  NOT NULL,
    move       JSONB NOT NULL,
    state      JSONB NOT NULL,
    created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (game_id) REFERENCES "game" (id)
);

CREATE TABLE "exp_snapshot" 
( 
    id                  UUID PRIMARY KEY, 
    user_id             UUID NOT NULL, 
    exp                 INT NOT NULL, 
    last_exp_id         UUID NOT NULL, 
    last_exp_created_at TIMESTAMPTZ NOT NULL, 
    created_at          TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP, 
    FOREIGN KEY (user_id) REFERENCES "user" (id), 
    FOREIGN KEY (last_exp_id) REFERENCES "exp" (id) 
); 