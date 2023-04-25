import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'environments/environment';
import { StoreDto } from '../models/storeDto';
import { Store } from '../models/store';


@Injectable({
  providedIn: 'root'
})
export class StoreService {

  constructor(private httpClient: HttpClient) { }


  getStoreList(): Observable<Store[]> {

    return this.httpClient.get<Store[]>(environment.getApiUrl + '/stores/getall')
  }

  getStoreDtoList(): Observable<StoreDto[]> {
    return this.httpClient.get<StoreDto[]>(environment.getApiUrl + '/stores/getdtolist')
  }

  getStoreById(id: number): Observable<Store> {
    return this.httpClient.get<Store>(environment.getApiUrl + '/stores/getbyid?id='+id)
  }

  getStoreDtoById(id: number): Observable<StoreDto> {
    return this.httpClient.get<StoreDto>(environment.getApiUrl + '/stores/getdtobyid?id='+id)
  }

  addStore(store: Store): Observable<any> {

    return this.httpClient.post(environment.getApiUrl + '/stores/', store, { responseType: 'text' });
  }

  updateStore(store: Store): Observable<any> {
    return this.httpClient.put(environment.getApiUrl + '/stores/', store, { responseType: 'text' });

  }

  deleteStore(id: number) {
    return this.httpClient.request('delete', environment.getApiUrl + '/stores/', { body: { id: id } });
  }


}