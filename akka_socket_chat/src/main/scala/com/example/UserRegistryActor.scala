package com.example

//#user-registry-actor
import akka.actor.{Actor, ActorLogging, Props}
import com.example.ChatUserHandler.AddUserToChatRoom

//message to Client
final case class ActionPerformed(description: String)

//#user-case-classes
final case class User(name: String, age: Int, countryOfResidence: String)
final case class Users(users: Set[User])
//#user-case-classes
final case class Chat(chatRooms: Set[ChatRoom])
final case class ChatRoom(id: Long, name: String, users: Set[User])

object ChatRegistryActor {
  final case object GetChatRooms
  final case object GetChatRoom
  final case class CreateChatRoom(chatRoom: ChatRoom)
  final case class DeleteChatRoom(chatRoom: ChatRoom)

  def props: Props = Props[ChatRegistryActor]
}
object ChatUserHandler {
  final case class AddUserToChatRoom(user: User, chatRoomId: Long)
  final case class RemoveUserFromChatRoom(user: User, chatRoomId: Long)
}
object UserRegistryActor {
//  final case class ActionPerformed(description: String)
  final case object GetUsers
  final case class CreateUser(user: User)
  final case class GetUser(name: String)
  final case class DeleteUser(name: String)

  def props: Props = Props[UserRegistryActor]
}
class ChatRegistryActor() extends Actor with ActorLogging {
  import ChatUserHandler._
  import ChatRegistryActor._

  val chat = Chat(Set.empty[ChatRoom])
  var users = Set.empty[User]

  private def performOnChatRoom(chatRoomId: Long, user: User, biConsumer: (ChatRoom, User) => Unit) = {
    chat.chatRooms.find(cr => chatRoomId == cr.id) match {
      case Some(chatRoom) => biConsumer(chatRoom, user)
      case None => sender() ! ActionPerformed("ChatRoom doesnt exist")
    }
  }
    def receive: Receive = {
    case CreateChatRoom(chatRoom) =>
      chat.chatRooms += chatRoom
    case AddUserToChatRoom(user, chatRoomId) =>
        performOnChatRoom(chatRoomId, user, (chatRoom: ChatRoom, user: User) => chatRoom.users += user )
    case RemoveUserFromChatRoom(user, chatRoomId) =>
        performOnChatRoom(chatRoomId, user, (chatRoom: ChatRoom, user: User) => chatRoom.users -= user )
      }


//    case CreateUser(user) =>
//      users += user
//      sender() ! ActionPerformed(s"User ${user.name} created.")
//    case GetUser(name) =>
//      sender() ! users.find(_.name == name)
//    case DeleteUser(name) =>
//      users.find(_.name == name) foreach { user => users -= user }
//      sender() ! ActionPerformed(s"User ${name} deleted.")
  }
}


class UserRegistryActor() extends Actor with ActorLogging {
  import UserRegistryActor._

  var users = Set.empty[User]

  def receive: Receive = {
    case GetUsers =>
      sender() ! Users(users.toSeq)
    case CreateUser(user) =>
      users += user
      sender() ! ActionPerformed(s"User ${user.name} created.")
    case GetUser(name) =>
      sender() ! users.find(_.name == name)
    case DeleteUser(name) =>
      users.find(_.name == name) foreach { user => users -= user }
      sender() ! ActionPerformed(s"User ${name} deleted.")
  }
}
//#user-registry-actor