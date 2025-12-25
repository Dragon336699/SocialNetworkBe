CREATE KEYSPACE IF NOT EXISTS fricon 
WITH replication = {'class': 'SimpleStrategy', 'replication_factor': '1'};

USE fricon;

CREATE TABLE IF NOT EXISTS user_feed_seen (
  user_id UUID,
  post_id UUID,
  feed_id UUID,
  seen_at TIMESTAMP,
  PRIMARY KEY ((user_id), feed_id)
);

CREATE TABLE IF NOT EXISTS user_feed_unseen (
  user_id UUID,
  post_id UUID,
  created_at TIMESTAMP,
  feed_id UUID,
  PRIMARY KEY ((user_id), created_at, feed_id)
)  WITH CLUSTERING ORDER BY (created_at DESC, feed_id ASC);

CREATE TABLE user_interaction_counter (
  user_id UUID,
  target_user_id UUID,
  view_count COUNTER,
  like_count COUNTER,
  search_count COUNTER,
  PRIMARY KEY ((user_id), target_user_id)
);

CREATE TABLE user_interaction_meta (
  user_id UUID,
  target_user_id UUID,
  last_interaction TIMESTAMP,
  PRIMARY KEY ((user_id), target_user_id)
);

