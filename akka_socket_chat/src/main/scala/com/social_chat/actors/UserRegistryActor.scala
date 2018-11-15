package com.social_chat.actors

//#user-registry-actor
import akka.actor.{Actor, ActorLogging, Props}

final case class User(id: Long, name: String)
final case class Users(users: Set[User])

object UserRegistryActor {

  final case class ActionPerformed(description: String)
  final case object GetUsers
  final case class CreateUser(user: User)
  final case class GetUser(name: String)
  final case class DeleteUser(id: Long, name: String)

  def props: Props = Props[UserRegistryActor]
}


class UserRegistryActor() extends Actor with ActorLogging {
  import UserRegistryActor._

  var users = Set.empty[User]

  def receive: Receive = {
    case GetUsers =>
      sender() ! Users(users)
    case CreateUser(user) =>
      users += user
      sender() ! ActionPerformed(s"User ${user.name} created.")
    case GetUser(id) =>
      sender() ! users.find(_.id == id)
    case DeleteUser(id, name) =>
      users = users.filter(_.id != id)
      sender() ! ActionPerformed(s"User ${name} deleted.")
  }
}
