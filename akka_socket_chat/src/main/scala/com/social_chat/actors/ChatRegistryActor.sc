package com.social_chat.actors


import akka.actor.{Actor, ActorLogging, Props}
import com.social_chat.actors.ChatUserTypeMessagesBus.{AddUserToChatRoom, RemoveUserFromChatRoom}


final case class Chat(chatRooms: Set[ChatRoom])
final case class ChatRoom(id: Long, name: String, users: Set[User])
final case class ChatMessage(userId: Long, message: String) {
  import java.util.Calendar
  val messageDate = Calendar.getInstance().getTimeInMillis
}

object ChatRegistryActor {
  final case class ActionPerformed(description: String)
  final case object GetChatRooms
  final case object GetChatRoom
  final case class CreateChatRoom(chatRoom: ChatRoom)
  final case class DeleteChatRoom(chatRoom: ChatRoom)

  def props: Props = Props[ChatRegistryActor]
}
class ChatRegistryActor() extends Actor with ActorLogging {
  import ChatRegistryActor._

  val chat = Chat(Set.empty[ChatRoom])
  var users = Set.empty[User]

  private def performOnChatRoom(chatRoomId: Long, user: User, biConsumer: (ChatRoom, User) => Unit) = {
    chat.chatRooms.find(_.id == chatRoomId) match {
      case Some(chatRoom) => biConsumer(chatRoom, user)
      case None => sender() ! ActionPerformed("ChatRoom doesnt exist")
    }
  }
  def receive: Receive = {
    case GetChatRooms =>
      sender() ! chat.chatRooms

    case CreateChatRoom(chatRoom) =>
      chat.chatRooms += chatRoom
      sender() ! ActionPerformed(s"ChatRoom ${chatRoom.name} created.")
    case AddUserToChatRoom(user, chatRoomId) =>
      performOnChatRoom(chatRoomId, user, (chatRoom: ChatRoom, user: User) => chatRoom.users += user )
    case RemoveUserFromChatRoom(user, chatRoomId) =>
      performOnChatRoom(chatRoomId, user, (chatRoom: ChatRoom, user: User) => chatRoom.users -= user )
  }

}