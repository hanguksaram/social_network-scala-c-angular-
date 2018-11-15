import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { filter, map } from 'rxjs/operators';
import { PaginatedResult } from '../_models/pagination';
import { Message } from '../_models/message';



@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  createHttpParams(page?, itemsPerPage?, userParams?, likesParam?, messageContainer?): HttpParams {
    let params = new HttpParams();
    if (page && itemsPerPage) {
      params = params.append('pageNumber', page)
        .append('pageSize', itemsPerPage);
    }
    if (userParams) {
      params = params.append('minAge', userParams.minAge)
        .append('maxAge', userParams.maxAge)
        .append('gender', userParams.gender)
        .append('orderBy', userParams.orderBy)  
    }

    if (messageContainer) {
      params = params.append('messageContainer', messageContainer);
    }
    if (likesParam === 'Likers') {
      params = params = params.append('likers', 'true')
    }
    if (likesParam === 'Likees') {
      params = params = params.append('likees', 'true')
    }
    return params
  }
  
  getUsers(page?, itemsPerPage?, userParams?, likesParam?): Observable<PaginatedResult<User[]>> {
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();
    const params = this.createHttpParams(page, itemsPerPage, userParams, likesParam);

    return this.http.get<User[]>(this.baseUrl + 'users', {observe: 'response', params}).
      pipe(
        map(response => {
        paginatedResult.result = response.body.filter(user => user.id !== environment.adminId);
        const paginatonHeader = response.headers.get('Pagination')
        if (paginatonHeader) {
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
        }
        return paginatedResult;
      }));
  }
  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }
  updateUser(id: Number, user: User) {
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }
  setMainPhoto(userId: number, id: Number) {
    return this.http.post(`${this.baseUrl}users/${userId}/photos/${id}/setMain`, {});
  }
  deletePhoto(userId: number, id: Number) {
    return this.http.delete(`${this.baseUrl}users/${userId}/photos/${id}`);
  }
  sendLike(id: Number, recipientId: number) {
    return this.http.post(`${this.baseUrl}users/${id}/like/${recipientId}` , {});
  }
  getMessages(id: Number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>()
    const params = this.createHttpParams(page, itemsPerPage, null, null, messageContainer)

    return this.http.get<Message[]>(`${this.baseUrl}users/${id}/messages/`, {observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;
          const pagingHeader = response.headers.get('Pagination');
          if (pagingHeader) {
            paginatedResult.pagination = JSON.parse(pagingHeader)
          }
          return paginatedResult
        })
      )
    
  }
  getMessageThread(id: number, recipientId: number) {
    return this.http.get<Message[]>(`${this.baseUrl}users/${id}/messages/thread/${recipientId}`)
  }
  sendMessage(id: number, message: Message) {
    return this.http.post(`${this.baseUrl}users/${id}/messages`, message)
    
  }


}
