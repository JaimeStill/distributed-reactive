import { Comment } from './comment';
import { Post } from './post';
import { User } from './user';

export interface Vote {
  id: number;
  voterId: number;
  type: string;
  up: boolean;

  voter?: User;
}

export interface CommentVote extends Vote {
  commentId: number;
  comment?: Comment;
}

export interface PostVote extends Vote {
  postId: number;
  post?: Post;
}
