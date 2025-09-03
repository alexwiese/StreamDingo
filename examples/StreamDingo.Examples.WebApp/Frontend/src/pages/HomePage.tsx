import { Link } from 'react-router-dom';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '../components/ui/card';
import { Button } from '../components/ui/button';

export default function HomePage() {
  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">
          StreamDingo Event Sourcing Example
        </h1>
        <p className="mt-2 text-lg text-gray-600">
          A demonstration of event sourcing with users, businesses, and relationships using StreamDingo library.
        </p>
      </div>

      <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
        <Card>
          <CardHeader>
            <CardTitle>Users</CardTitle>
            <CardDescription>
              Manage user accounts with names, emails, and phone numbers.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Link to="/users">
              <Button className="w-full">View Users</Button>
            </Link>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Businesses</CardTitle>
            <CardDescription>
              Manage business entities with details like industry, address, and website.
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Link to="/businesses">
              <Button className="w-full">View Businesses</Button>
            </Link>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Relationships</CardTitle>
            <CardDescription>
              Manage relationships between users and businesses (employees, partners, etc.).
            </CardDescription>
          </CardHeader>
          <CardContent>
            <Link to="/relationships">
              <Button className="w-full">View Relationships</Button>
            </Link>
          </CardContent>
        </Card>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>About Event Sourcing with StreamDingo</CardTitle>
          <CardDescription>
            How this example demonstrates event sourcing principles
          </CardDescription>
        </CardHeader>
        <CardContent>
          <ul className="space-y-2 text-sm text-gray-600">
            <li>• <strong>Events</strong>: All changes are captured as events (UserCreated, BusinessUpdated, etc.)</li>
            <li>• <strong>Event Handlers</strong>: Pure functions transform snapshots when events are applied</li>
            <li>• <strong>Snapshots</strong>: Current state is maintained as snapshots with hash verification</li>
            <li>• <strong>Replay</strong>: System can replay events to rebuild state from any point in time</li>
            <li>• <strong>Integrity</strong>: Hash-based verification ensures data hasn't been tampered with</li>
            <li>• <strong>Relationships</strong>: Cross-entity relationships are maintained through event sourcing</li>
          </ul>
        </CardContent>
      </Card>
    </div>
  );
}