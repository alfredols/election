-- Table: filtered_voter
CREATE TABLE IF NOT EXISTS "filtered_voter" (
 "id"  integer constraint "PK_filtered_voter" primary key autoincrement
, "identifier" nvarchar(25) null
, "name" nvarchar(200) null, tre_zone varchar (10)
, "has_voted" bit null
, "election_id" int not null
, "section_id" int not null
);

-- Table: vote
CREATE TABLE IF NOT EXISTS "vote" (
 "id"  integer constraint "PK_vote" primary key autoincrement

, "created_on" datetime null

, "nickname_station" nvarchar(2147483647) null

, "candidate_id" int null

);

-- Table: candidate_picture
CREATE TABLE IF NOT EXISTS "candidate_picture" (
 "id"  integer constraint "PK_candidate_picture" primary key autoincrement
, "data" blob null
, "candidate_id" int not null
);

CREATE TABLE IF NOT EXISTS "report" (
 "id"  integer constraint "PK_report" primary key autoincrement
, "identifier" varchar(100) not null
, "path" text not null,  created_on DATETIME not null
);

-- Index: filtered_voter_IX_election_id
CREATE INDEX IF NOT EXISTS "filtered_voter_IX_election_id" ON "filtered_voter" ("election_id");

-- Index: filtered_voter_IX_section_id
CREATE INDEX IF NOT EXISTS "filtered_voter_IX_section_id" ON "filtered_voter" ("section_id");

-- Index: vote_IX_candidate_id
CREATE INDEX IF NOT EXISTS "vote_IX_candidate_id" ON "vote" ("candidate_id");

-- Index: candidate_picture_IX_candidate_id
CREATE INDEX IF NOT EXISTS "candidate_picture_IX_candidate_id" ON "candidate_picture" ("candidate_id");