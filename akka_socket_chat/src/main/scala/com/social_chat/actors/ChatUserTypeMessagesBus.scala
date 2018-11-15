package com.social_chat.actors

object ChatUserTypeMessagesBus {
    final case class AddUserToChatRoom(user: User, chatRoomId: Long)
    final case class RemoveUserFromChatRoom(user: User, chatRoomId: Long)
  }

