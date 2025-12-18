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