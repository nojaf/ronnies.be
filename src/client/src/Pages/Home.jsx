import React from 'react';
import { useRonniesList } from "../bin/Hooks"
import {Table} from "reactstrap";

const Home = () => {
    const ronnies = useRonniesList()
    const ronnyRow = ({ id, name, date}) => {
        return <tr  key={id}>
            <td>{name}</td>
            <td>{date}</td>
        </tr>
    }
    return (
        <div>
            <h1>Ronnies.be</h1>
            <Table>
                <thead>
                    <tr>
                        <th>Naam</th>
                        <th>Toegevoegd op</th>
                    </tr>
                </thead>
                <tbody>
                {ronnies.map(ronnyRow)}
                </tbody>
            </Table>
        </div>
    );
};

export default Home;