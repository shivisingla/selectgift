import { Injectable } from '@angular/core'
import { HttpClient } from '@angular/common/http'

@Injectable()
export class ApiService {

    constructor(private http: HttpClient) {}

    getRelations() {
        return this.http.get('https://localhost:44386/api/person/relations');
    }

    getOccasions() {
        return this.http.get('https://localhost:44386/api/person/occasions');
    }

    getInterests() {
        return this.http.get('https://localhost:44386/api/person/interests');
    }

    postPerson(person) {
        return this.http.post('https://localhost:44386/api/person', person);
    }

    postPersonWithPresent(person) {
        return this.http.post('https://localhost:44386/api/person/AddPerson', person);
    }
}